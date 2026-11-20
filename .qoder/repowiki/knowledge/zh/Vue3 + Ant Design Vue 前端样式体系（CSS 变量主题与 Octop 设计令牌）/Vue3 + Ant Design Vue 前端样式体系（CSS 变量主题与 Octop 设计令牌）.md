---
kind: frontend_style
name: Vue3 + Ant Design Vue 前端样式体系（CSS 变量主题与 Octop 设计令牌）
category: frontend_style
scope:
    - '**'
source_files:
    - vue/kevin.web.vue/package.json
    - vue/kevin.web.vue/vue.config.js
    - vue/kevin.web.vue/src/css/design-tokens.css
    - vue/kevin.web.vue/src/css/themes.css
    - vue/kevin.web.vue/src/css/buttons.css
    - vue/kevin.web.vue/src/css/enterprise-surface.css
    - vue/kevin.web.vue/src/css/CardTable.css
    - vue/kevin.web.vue/src/css/MyAIChat.css
    - vue/kevin.web.vue/src/css/MyTable.css
    - vue/kevin.web.vue/src/css/UserList.css
    - vue/kevin.web.vue/src/css/UserProfile.css
    - vue/kevin.web.vue/src/css/UserRole.css
    - vue/kevin.web.vue/src/css/kevinHome.css
    - vue/kevin.web.vue/src/css/kevinLogin.css
    - vue/kevin.web.vue/src/css/management.css
    - vue/kevin.web.vue/src/css/pagination-enterprise.css
---

## 1. 系统与方法论

该仓库的前端位于 `vue/kevin.web.vue/`，基于 **Vue 3** (`vue@^3.2.13`) + **Ant Design Vue 4** (`ant-design-vue@^4.2.6`)，构建工具为 **Vue CLI 5** (`@vue/cli-service ~5.0.0`)。样式方案是「Ant Design 组件库 + 全局 CSS 变量主题 + 业务级独立 .css 文件」的组合：

- 组件层使用 Ant Design Vue 的 `a-*` 组件。
- 品牌色、背景、阴影、圆角等视觉值通过 CSS 自定义属性集中管理（`design-tokens.css`），由 `themes.css` 提供多套品牌变体。
- 业务页面各自维护独立的 `.css` 文件（`src/css/` 下约 14 个文件），并通过 `enterprise-surface.css` 统一将历史深色玻璃风页面适配到浅色企业表面。
- 没有使用 Sass/Less/Tailwind；所有样式均为原生 CSS。

## 2. 关键文件

| 文件 | 作用 |
|---|---|
| `vue/kevin.web.vue/package.json` | 声明 `ant-design-vue`、`vue`、`vue-router`、`axios`、`codemirror` 等依赖及 Vue CLI 脚本 |
| `vue/kevin.web.vue/vue.config.js` | Vue CLI 配置，含 devServer 代理 `/VarApi` → `http://localhost:9901` |
| `vue/kevin.web.vue/src/css/design-tokens.css` | 设计令牌根文件，定义 `--fn-color-*` / `--fn-bg-*` / `--fn-text-*` / `--fn-shadow-*` / `--fn-radius-*` / `--fn-transition-*` 等全部语义化 CSS 变量，并桥接旧变量名 `--accent` / `--page-bg` / `--sider-bg` 等 |
| `vue/kevin.web.vue/src/css/themes.css` | 7 套品牌主题（`.theme-enterprise` / `.theme-blackblue` / `.theme-default` / `.theme-green` / `.theme-purple` / `.theme-darkblue` / `.theme-simple-white`），每个只覆盖 `--fn-color-brand` 与 `--fn-color-brand-hover` |
| `vue/kevin.web.vue/src/css/buttons.css` | 覆盖 Ant Design 组件（`ant-btn-primary`、`ant-input`、`ant-select`、`ant-checkbox`、`ant-radio`、`ant-picker`、`ant-switch`）的主题色，统一走 `var(--accent)` |
| `vue/kevin.web.vue/src/css/enterprise-surface.css` | 以 `@import './design-tokens.css'` 引入令牌，并将历史深色玻璃风页面（用户管理、AI 聊天、消息列表等）强制适配为白底浅灰边框的企业风格 |
| `vue/kevin.web.vue/src/css/*.css`（CardTable、MyAIChat、MyTable、UserList、UserProfile、UserRole、kevinHome、kevinLogin、management、pagination-enterprise 等） | 各业务模块的独立样式文件 |

## 3. 架构与约定

### 设计令牌分层
`design-tokens.css` 采用三层命名空间（见文件顶部注释）：
- **基础层**：`--fn-bg-*` / `--fn-text-*` / `--fn-border-*`
- **语义层**：`--fn-color-success` / `--fn-color-danger` / `--fn-color-warning` / `--fn-color-info`
- **区域层**：`--fn-sidebar-*` / `--fn-assistant-bubble-*` / `--fn-row-selected-*`

品牌派生值一律通过 `color-mix(in srgb, var(--fn-color-brand) X%, transparent)` 自动推导（如 `--fn-color-brand-light`、`--fn-bg-hover`、`--fn-bg-active`、`--fn-bg-selected`、`--fn-color-brand-shadow`、`--fn-focus-ring`），因此换品牌只需改 `--fn-color-brand` 与 `--fn-color-brand-hover`。

### 主题切换机制
- 默认品牌色为 Elegant Rose `#e85d75`（即 `theme-simple-white`）。
- 主题类加在承载布局的 `.layout-container` 元素上；由于 CSS 自定义属性在声明处求值，`themes.css` 中 `.layout-container` 块会重新计算所有品牌派生值与桥接变量，使历史 CSS 中的 `var(--accent)` 等引用随主题变化。
- 主题选择菜单的色块预览通过 `.theme-preview.theme-xxx` 类实现。
- 已知限制：antd 弹层渲染到 `<body>` 下，仍吃 `:root` 默认品牌色，无法跟随 `.layout-container` 上的主题类。

### Ant Design 主题覆盖策略
`buttons.css` 通过 `.layout-container .ant-*` 选择器覆盖 Ant Design 组件的默认主题色，全部走 `var(--accent)` / `var(--accent-hover)` / `var(--accent-soft)` 三个桥接变量，确保按钮、输入框、选择器、复选框、单选框、日期选择器、开关等统一受主题控制。

### 企业表面适配
`enterprise-surface.css` 针对历史页面的深色玻璃风（`backdrop-filter`、白字）进行逆向适配：把卡片、表格、头像、气泡、搜索框等强制改为白底 + `#f0f0f0` 边框 + `rgba(0,0,0,0.88)` 文字，同时保留 AI 气泡形状（尖角逻辑在 `MyAIChat.css`）。大量规则使用 `!important` 覆盖历史样式。

### 响应式策略
未发现媒体查询或断点；项目未使用 Tailwind 等响应式框架，也未发现专门的响应式 CSS 文件。Ant Design Vue 组件自身承担部分响应式行为。

## 4. 约定与约束

- **品牌色入口唯一**：新增主题时只在 `themes.css` 中添加一个 `.theme-xxx` 类，仅覆盖 `--fn-color-brand` 与 `--fn-color-brand-hover`，其余派生值由 `design-tokens.css` 的 `.layout-container` 块自动重算（themes.css 第 4–8 行注释明确说明此契约）。
- **业务样式按页面拆分**：每个业务模块在 `src/css/` 下拥有独立 `.css` 文件，不混入组件内部样式。
- **Ant Design 组件主题必须通过 `var(--accent)` 系列变量覆盖**：`buttons.css` 中所有 `.layout-container .ant-*` 规则都走 `--accent` / `--accent-hover` / `--accent-soft`，禁止直接写死颜色值。
- **历史样式兼容**：`design-tokens.css` 底部保留 `--accent` / `--page-bg` / `--sider-bg` / `--header-bg` / `--content-surface` / `--text-strong` / `--text-muted` / `--hover-bg` / `--danger` 等旧变量名作为桥接，保证已有 CSS 无需改动即可消费新令牌。
- **企业表面强制规范**：`enterprise-surface.css` 对 `content-wrapper` 下的历史页面使用大量 `!important` 强制白底浅灰边框，这是为了统一历史深色玻璃风页面与企业浅色表面的视觉一致性（见文件首行注释）。
- **构建产物**：通过 `vue-cli-service build` 输出到 `dist/`，Dockerfile 配合 nginx 部署静态资源。

## 5. 不适用性判断

本仓库是一个 .NET Core 后端 + Vue 3 前端的混合项目，前端确实存在完整的样式体系（Ant Design Vue + CSS 变量主题 + 业务 .css），因此 `frontend_style` 类别适用且证据充分（多个主题文件、设计令牌、组件覆盖规则均清晰可查）。