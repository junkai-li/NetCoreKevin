using FakeItEasy;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Domain.Share.Dtos.User;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using Kevin.Web.Basics.Controllers;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Kevin.Unit.Kevin.WebBasics
{
    /// <summary>
    /// 用户导出回归测试（TASK-04）：
    /// 本次需求为非目标「导出列不变」，用真实 Excel 输出流校验列头与顺序未被新增字段污染。
    /// </summary>
    public class UserControllerExportColumnTests
    {
        /// <summary>导出列期望顺序（UserController.ExportGetSysUserList 内硬编码 10 列）</summary>
        private static readonly string[] ExpectedTitles =
        {
            "用户名", "昵称", "角色", "岗位", "部门", "手机号", "邮箱", "创建时间", "状态", "最近登陆时间"
        };

        [Fact]
        public async Task ExportGetSysUserList_ShouldKeepTenColumnsAndOrderUnchanged()
        {
            // Arrange：返回一个已锁定用户，确认新字段存在也不会被导出
            var userService = A.Fake<IUserService>();
            A.CallTo(() => userService.GetSysUserList(A<dtoPagePar<dtoUserPar>>._))
                .Returns(Task.FromResult(new dtoPageData<dtoUser>
                {
                    total = 1,
                    data = new List<dtoUser>
                    {
                        new dtoUser
                        {
                            Id = 1,
                            Name = "tester",
                            NickName = "测试账号",
                            Phone = "13800000000",
                            Email = "tester@example.com",
                            Status = true,
                            CreateTime = new DateTime(2026, 1, 2, 3, 4, 5),
                            RecentLoginTime = new DateTime(2026, 1, 3, 4, 5, 6),
                            FailedCount = 6,
                            LockoutEnd = DateTime.Now.AddMinutes(10),
                            // 导出动作对 Roles/Positions 直接 string.Join，需保证集合非 null（与 GetSysUserList 产出形态一致）
                            Roles = new List<dtoRole>(),
                            Positions = new List<PositionDto>(),
                        }
                    }
                }));

            var controller = new UserController(userService);

            // Act
            var result = await controller.ExportGetSysUserList(new dtoPagePar<dtoUserPar>());

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("系统用户列表信息.xlsx", fileResult.FileDownloadName);
            Assert.NotNull(fileResult.FileStream);

            using var workbook = WorkbookFactory.Create(fileResult.FileStream);
            var sheet = workbook.GetSheetAt(0);
            var titleRow = sheet.GetRow(0);
            var titles = Enumerable.Range(0, titleRow.LastCellNum)
                .Select(i => titleRow.GetCell(i)?.StringCellValue?.Trim() ?? "")
                .ToArray();

            Assert.Equal(ExpectedTitles, titles);
            // 新增的展示字段不得出现在导出列中
            Assert.DoesNotContain("锁定时间", titles);
            Assert.DoesNotContain("失败次数", titles);
            // 数据行应写入「最近登陆时间」列
            var dataRow = sheet.GetRow(1);
            Assert.Equal(ExpectedTitles.Length, dataRow.LastCellNum);
            Assert.Equal("2026-01-03", dataRow.GetCell(9)?.StringCellValue);
        }
    }
}
