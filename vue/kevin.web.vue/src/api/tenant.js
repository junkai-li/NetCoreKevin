import request from '@/utils/http';

// 获取租户分页数据
export function getTenantPage(params) {
    return request({
        url: '/api/Tenant/GetPageData',
        method: 'post',
        data: params
    });
}

// 新增租户
export function createTenant(data) {
    return request({
        url: '/api/Tenant/CreateAsync',
        method: 'post',
        data: data
    });
}

// 编辑租户
export function editTenant(data) {
    return request({
        url: '/api/Tenant/EidtAsync',
        method: 'post',
        data: data
    });
}

// 删除租户
export function deleteTenant(id) {
    return request({
        url: '/api/Tenant/DeleteAsync',
        method: 'delete',
        data: { id: id }
    });
}

// 设置租户有效
export function activateTenant(id) {
    return request({
        url: '/api/Tenant/Active',
        method: 'post',
        data: { id: id }
    });
}

// 设置租户无效
export function deactivateTenant(id) {
    return request({
        url: '/api/Tenant/Inactive',
        method: 'post',
        data: { id: id }
    });
}