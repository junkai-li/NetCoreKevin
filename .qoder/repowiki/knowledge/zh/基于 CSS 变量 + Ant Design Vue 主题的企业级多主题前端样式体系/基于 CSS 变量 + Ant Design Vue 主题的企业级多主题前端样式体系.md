---
kind: frontend_style
name: 基于 CSS 变量 + Ant Design Vue 主题的企业级多主题前端样式体系
category: frontend_style
scope:
    - '**'
source_files:
    - vue/kevin.web.vue/package.json
    - vue/kevin.web.vue/vue.config.js
    - vue/kevin.web.vue/src/main.js
    - vue/kevin.web.vue/src/App.vue
    - vue/kevin.web.vue/src/css/themes.css
    - vue/kevin.web.vue/src/css/buttons.css
    - vue/kevin.web.vue/src/css/enterprise-surface.css
    - vue/kevin.web.vue/src/css/pagination-enterprise.css
    - vue/kevin.web.vue/src/css/kevinHome.css
    - vue/kevin.web.vue/src/css/management.css
    - vue/kevin.web.vue/src/pages/kevinHome.vue
---

## 1. 系统/技术栈
- 前端框架：Vue 3（`vue@^3.2.13`）+ Vue Router 4，构建工具为 `@vue/cli-service`（`vue.config.js` 配置开发代理与依赖转译）。
- UI 组件库：**Ant Design Vue 4**（`ant-design-vue@^4.2.6`），通过全局 `a-config-provider` 注入主题，并引入其 reset CSS（`ant-design-vue/dist/reset.css`）作为基础样式。
- 编辑器：CodeMirror 6（含 JavaScript/Markdown 语言包、One Dark 主题）用于 AI 相关代码编辑场景。
- 无 CSS 预处理器（未见 `.scss`/`.less`/`tailwind.config.*`），全部使用原生 CSS + CSS 自定义属性（CSS Variables）实现主题化。

## 2. 关键文件
- `vue/kevin.web.vue/src/main.js`：应用入口，注册 Antd、Router，并全局引入 `enterprise-surface.css`、`pagination-enterprise.css`。
- `vue/kevin.web.vue/src/App.vue`：根组件，通过 `a-config-provider` 将 Ant Design 的 `colorPrimary` 绑定到 CSS 变量 `--accent`，使组件主题色与 CSS 变量联动。
- `vue/kevin.web.vue/src/css/themes.css`：**核心设计令牌集中地**，定义 `layout-container` 下的 CSS 变量（`--page-bg`、`--sider-bg`、`--accent`、`--accent-hover`、`--accent-soft`、`--header-bg`、`--content-surface`、`--text-strong`、`--danger` 等），并以 `theme-enterprise`、`theme-blackblue`、`theme-default`、`theme-green`、`theme-purple`、`theme-darkblue`、`theme-simple-white` 七个类名提供多套主题。
- `vue/kevin.web.vue/src/css/buttons.css`：统一覆盖 Antd 按钮、输入框、选择器、开关、日期选择器等组件的聚焦态与强调色，全部引用 `--accent` / `--accent-hover` / `--accent-soft` / `--danger`。
- `vue/kevin.web.vue/src/css/enterprise-surface.css`：把历史深色玻璃风页面统一成浅色企业风格卡片/表格/搜索栏/AI 对话区，强制白底、浅灰边框与可读文本色。
- `vue/kevin.web.vue/src/css/pagination-enterprise.css`：全站分页统一样式（圆角、边框、激活态高亮）。
- `vue/kevin.web.vue/src/css/kevinHome.css`：布局骨架（侧边栏、头部、内容区、页脚、标签页导航），大量使用 `var(--xxx)` 变量。
- `vue/kevin.web.vue/src/pages/kevinHome.vue`：主题切换入口，通过给根节点添加/移除 `theme-*` 类名切换主题，并把当前主题持久化到 localStorage；同时根据主题判断 Antd 暗黑模式。

## 3. 架构与设计约定
- **设计令牌层**：所有颜色、背景、边框、文字色集中在 `themes.css` 的 CSS 变量中，业务组件不直接写死色值，而是通过 `var(--accent)` 等变量消费。
- **主题切换机制**：在根元素上挂载 `theme-enterprise` / `theme-blackblue` / `theme-default` / `theme-green` / `theme-purple` / `theme-darkblue` / `theme-simple-white` 七种主题类。`App.vue` 监听 `--accent` 变化并同步到 Antd 的 `colorPrimary`，保证 Antd 组件与 CSS 变量主题一致。
- **组件样式策略**：对 Antd 组件采用“全局覆盖”方式（如 `.layout-container .ant-btn-primary`、`.content-wrapper .ant-table`），而非按模块隔离的 BEM/CSS Modules。这是因为项目规模较小且追求快速统一外观。
- **页面结构约定**：`kevinHome.css` 定义了统一的 `.layout-container > main-layout > sider/header/content-wrapper/footer` 布局容器，页面组件只需放入 `content-wrapper` 即可继承布局样式。
- **响应式**：通过 `@media (max-width: 768px)` 在 `kevinHome.css`、`management.css` 中对侧边栏、头部、工具栏做折叠与间距调整，未使用 CSS Grid/Flexbox 媒体查询以外的方案。
- **字体与排版**：根字体栈为 `-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', 'PingFang SC', 'Microsoft YaHei', sans-serif`，中文优先苹方/微软雅黑。

## 4. 约定与约束
- **主题变量命名规范**：新增主题时需在 `themes.css` 中声明完整变量集（`--page-bg`、`--sider-bg`、`--accent`、`--accent-hover`、`--accent-soft`、`--header-bg`、`--content-surface`、`--text-strong`、`--text-muted`、`--danger` 等），并在对应 `theme-xxx` 块内覆盖。
- **强调色唯一来源**：全局强调色必须通过 `--accent` 变量使用，禁止在业务 CSS 中硬编码 `#1677ff` 等色值（`buttons.css`、`enterprise-surface.css` 已体现该约束）。
- **Antd 主题色同步**：新增主题后需确保 `App.vue` 能读取新的 `--accent` 并更新 `antdTheme.token.colorPrimary`，否则 Antd 组件会偏离主题。
- **页面容器约定**：业务页面应包裹在 `.content-wrapper` 中，以便 `enterprise-surface.css` 的浅色企业风格规则生效。
- **构建产物**：通过 `vue-cli-service build` 打包，`browserslist` 目标为 `> 1%, last 2 versions, not dead, not ie 11`，即不支持 IE 11。
- **无 CSS 预处理/原子化**：仓库未发现 Sass/Less/Tailwind 配置，样式组织以“全局 CSS 文件 + 少量组件内 `<style>`”为主，未采用 CSS Modules 或 scoped 样式隔离策略。