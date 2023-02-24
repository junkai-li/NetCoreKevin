using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.Extension
{
    /// <summary>
    /// 电话号1开头11位的数字
    /// </summary>
    public class PhoneRegularExpression : RegularExpressionAttribute
    {
        public PhoneRegularExpression() : base(@"^1[0-9]{10}$") { }
    }
    /// <summary>
    /// 身份证18位字母+数字
    /// </summary>
    public class IdCardNumRegularExpression : RegularExpressionAttribute
    {
        public IdCardNumRegularExpression() : base(@"^[A-Za-z0-9]{18}$") { }

    }
    /// <summary>
    /// 金额（整数或者小数位2位）
    /// </summary>
    public class MoneyPoint2RegularExpression : RegularExpressionAttribute
    {
        //^(([1-9]{1}\d*)|(0{1}))(\.\d{0,2})?$
        public MoneyPoint2RegularExpression() : base(@"^[1-9]{1}\d*$|^([0-9]*)(\.\d{0,2})$") { }
    }
    /// <summary>
    /// 金额（整数或者小数位2位）
    /// </summary>
    public class MoneyZeroPoint2RegularExpression : RegularExpressionAttribute
    {
        //^(([1-9]{1}\d*)|(0{1}))(\.\d{0,2})?$
        public MoneyZeroPoint2RegularExpression() : base(@"^[0-9]{1}\d*$|^([0-9]*)(\.\d{0,2})$") { }
    }
    /// <summary>
    /// 整数
    /// </summary>
    public class IntRegularExpression : RegularExpressionAttribute
    {
        //^(([1-9]{1}\d*)|(0{1}))(\.\d{0,2})?$
        public IntRegularExpression() : base(@"^[0-9]\d*$") { }
    }
}
