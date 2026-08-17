---
kind: frontend_style
name: 基于 Ant Design Vue + CSS 变量的企业级主题系统
category: frontend_style
scope:
    - '**'
source_files:
    - vue/kevin.web.vue/src/main.js
    - vue/kevin.web.vue/src/css/themes.css
    - vue/kevin.web.vue/src/css/buttons.css
    - vue/kevin.web.vue/src/css/enterprise-surface.css
    - vue/kevin.web.vue/src/css/pagination-enterprise.css
    - vue/kevin.web.vue/src/css/management.css
    - vue/kevin.web.vue/src/pages/kevinHome.vue
    - vue/kevin.web.vue/package.json
    - vue/kevin.web.vue/vue.config.js
---

## 1. 使用的系统与工具

- **UI 组件库**：Ant Design Vue 4.x（`ant-design-vue`），通过 `app.use(Antd)` 全局注册，并引入其重置样式 `ant-design-vue/dist/reset.css`。
- **构建与运行**：Vue CLI 5（`@vue/cli-service`），通过 `vue.config.js` 配置开发代理与依赖转译。
- **样式语言**：原生 CSS（无 Sass/Less/PostCSS 预处理），所有样式以 `.css` 文件形式组织在 `src/css/` 下。
- **运行时主题切换**：通过给根布局容器 `<a-layout class="layout-container" :class="currentTheme">` 动态挂载主题类名实现，无需重新编译。

## 2. 关键文件

- `vue/kevin.web.vue/src/main.js`：应用入口，全局注册 Ant Design Vue、引入全局样式与 dayjs 中文。
- `vue/kevin.web.vue/src/css/themes.css`：**设计令牌中心**，定义 `--page-bg`、`--sider-bg`、`--accent`、`--accent-hover`、`--accent-soft`、`--header-bg`、`--content-surface`、`--text-strong`、`--text-muted`、`--danger` 等 CSS 自定义属性，并以 `.theme-enterprise`、`.theme-blackblue`、`.theme-default`、`.theme-green`、`.theme-purple`、`.theme-darkblue`、`.theme-simple-white` 七套主题覆盖这些变量。
- `vue/kevin.web.vue/src/css/buttons.css`：统一覆盖 Ant Design 组件的强调色、聚焦态、选中态，全部绑定到 `--accent` / `--accent-hover` / `--accent-soft` 变量。
- `vue/kevin.web.vue/src/css/enterprise-surface.css`：将历史深色玻璃风页面统一为浅色企业风格卡片、表格、搜索框、AI 对话区，大量使用 `!important` 覆盖 Ant Design 默认样式。
- `vue/kevin.web.vue/src/css/pagination-enterprise.css`：统一分页器外观，固定圆角、边框色与激活色。
- `vue/kevin.web.vue/src/css/management.css`：管理后台通用卡片、工具栏、表格容器的布局与响应式样式（含 `@media (max-width: 768px)`）。
- `vue/kevin.web.vue/src/pages/kevinHome.vue`：布局根组件，挂载当前主题类名并提供主题下拉菜单。

## 3. 架构与设计约定

### 3.1 设计令牌（Design Tokens）
所有颜色、背景、强调色均通过 CSS 变量集中声明在 `themes.css` 的 `.layout-container` 上，各主题类仅覆盖需要变化的变量。业务样式通过 `var(--accent)`、`var(--accent-hover)`、`var(--accent-soft)`、`var(--page-bg)`、`var(--sider-bg)` 等引用，从而保证同一主题下全站视觉一致。

### 3.2 主题体系
- 提供 7 套预设主题：企业蓝（默认）、墨黑、灰蓝、绿色、紫色、深海蓝、企业简约白。
- 每套主题包含侧栏底色、强调色、悬停色、弱强调色、图标色等一组变量。
- 主题切换通过 `kevinHome.vue` 中 `switchTheme(name)` 动态替换根元素 class 实现，预览色块由 `.theme-preview` 类生成。

### 3.3 组件样式策略
- 对 Ant Design 组件采用**全局覆盖**方式：在 `buttons.css`、`pagination-enterprise.css`、`enterprise-surface.css` 中以 `.layout-container .ant-*`、`.content-wrapper .ant-*` 等高优先级选择器覆盖默认样式，大量使用 `!important` 确保覆盖生效。
- 业务组件样式按页面/模块拆分到独立 CSS 文件（如 `MyAIChat.css`、`UserList.css`、`UserProfile.css`、`UserRole.css`、`kevinHome.css`、`kevinLogin.css`、`CardTable.css`、`MyTable.css`），而非内联或 scoped 单文件。

### 3.4 布局与响应式
- 主布局基于 Ant Design Layout（`a-layout`、`a-layout-sider`、`a-layout-header`），侧边栏固定宽度 256px，可折叠至 80px。
- 响应式断点集中在 `management.css` 中 `@media (max-width: 768px)`，将工具栏、头部操作区改为纵向堆叠。

## 4. 约定与约束

- **主题变量命名规范**：新增主题时必须定义完整的变量集合（`--page-bg`、`--sider-bg`、`--accent`、`--accent-hover`、`--accent-soft`、`--header-icon-color`、`--text-strong`、`--text-muted`、`--danger` 等），并在 `themes.css` 中添加对应 `.theme-xxx` 类及 `.theme-preview.theme-xxx` 预览块。
- **强调色统一来源**：所有按钮、输入框聚焦、开关、复选框、单选框、日期选择器等交互态必须使用 `var(--accent)` / `var(--accent-hover)` / `var(--accent-soft)`，禁止硬编码十六进制色值。
- **浅色内容区强制**：`enterprise-surface.css` 要求所有业务卡片、表格、输入框、AI 对话区在浅色内容区内使用白色背景与 `#f0f0f0` 边框，并通过 `backdrop-filter: none !important` 移除历史玻璃模糊效果。
- **Ant Design 覆盖方式**：通过 `.layout-container` 和 `.content-wrapper` 作为作用域前缀覆盖 Ant Design 默认样式，新增覆盖需遵循相同前缀模式。
- **构建产物**：无 Tailwind、Sass、Less 等预处理；样式最终由 Vue CLI 打包进静态资源，生产环境关闭 `productionTip`。
- **浏览器兼容**：`browserslist` 配置为 `> 1%, last 2 versions, not dead, not ie 11`，因此未使用 IE 兼容 polyfill（除 `resize-observer-polyfill` 外）。