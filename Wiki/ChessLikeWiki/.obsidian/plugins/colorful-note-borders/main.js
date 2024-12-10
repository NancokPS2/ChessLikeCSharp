/*
THIS IS A GENERATED/BUNDLED FILE BY ESBUILD
if you want to view the source, please visit the github repository of this plugin
*/

var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/main.ts
var main_exports = {};
__export(main_exports, {
  default: () => ColorfulNoteBordersPlugin
});
module.exports = __toCommonJS(main_exports);
var import_obsidian2 = require("obsidian");

// src/settingsTab.ts
var import_obsidian = require("obsidian");
var DEFAULT_SETTINGS = {
  colorRules: [
    {
      id: "inbox-ffb300",
      value: "Inbox",
      type: "folder" /* Folder */,
      color: "#ffb300"
    },
    {
      id: "frontmatter-public-499749",
      value: "category: public",
      type: "frontmatter" /* Frontmatter */,
      color: "#499749"
    },
    {
      id: "frontmatter-private-c44545",
      value: "category: private",
      type: "frontmatter" /* Frontmatter */,
      color: "#c44545"
    }
  ]
};
var SettingsTab = class extends import_obsidian.PluginSettingTab {
  constructor(app, plugin) {
    super(app, plugin);
    this.plugin = plugin;
  }
  display() {
    let { containerEl } = this;
    containerEl.empty();
    containerEl.createEl("h1", { text: "Colorful Note Borders Settings" });
    const headerRow = containerEl.createEl("div", { cls: "cnb-rule-settings-header-row" });
    headerRow.createEl("span", { text: "Rule Type", cls: "cnb-rule-settings-column-rule-type" });
    headerRow.createEl("span", { text: "Value", cls: "cnb-rule-settings-column-rule-value" });
    headerRow.createEl("span", { text: "Color", cls: "cnb-rule-settings-column-rule-color" });
    headerRow.createEl("span", { text: "", cls: "cnb-rule-settings-column-rule-button" });
    const rulesContainer = containerEl.createEl("div", { cls: "cnb-rules-container" });
    this.plugin.settings.colorRules.forEach((rule, index) => this.addRuleSetting(rulesContainer, rule, index));
    new import_obsidian.ButtonComponent(containerEl).setButtonText("Add new rule").onClick(() => {
      const newRule = {
        id: Date.now().toString(),
        value: "",
        type: "folder" /* Folder */,
        color: "#000000"
      };
      this.plugin.settings.colorRules.push(newRule);
      this.addRuleSetting(rulesContainer, newRule);
      this.plugin.saveSettings();
    });
  }
  addRuleSetting(containerEl, rule, index = this.plugin.settings.colorRules.length - 1) {
    const ruleSettingDiv = containerEl.createEl("div", { cls: "cnb-rule-settings-row" });
    new import_obsidian.Setting(ruleSettingDiv).setClass("cnb-rule-setting-item").addDropdown((dropdown) => {
      dropdown.addOption("folder" /* Folder */, "Folder");
      dropdown.addOption("frontmatter" /* Frontmatter */, "Frontmatter");
      dropdown.setValue(rule.type);
      dropdown.onChange((value) => {
        rule.type = value;
        this.plugin.saveSettings();
      });
      dropdown.selectEl.classList.add("cnb-rule-type-dropdown");
    });
    new import_obsidian.Setting(ruleSettingDiv).setClass("cnb-rule-setting-item").addText((text) => {
      text.setPlaceholder("Enter rule value");
      text.setValue(rule.value);
      text.onChange((value) => {
        rule.value = value;
        this.plugin.saveSettings();
      });
      text.inputEl.classList.add("cnb-rule-value-input");
    });
    const colorSetting = new import_obsidian.Setting(ruleSettingDiv).setClass("cnb-rule-setting-item");
    const colorInput = new import_obsidian.TextComponent(colorSetting.controlEl).setPlaceholder("Enter color hex code").setValue(rule.color);
    colorInput.inputEl.classList.add("cnb-rule-setting-item-text-input");
    const picker = new import_obsidian.ColorComponent(colorSetting.controlEl).setValue(rule.color).onChange((color) => {
      rule.color = color;
      colorInput.setValue(color);
      this.plugin.saveSettings();
    });
    colorInput.onChange((value) => {
      if (/^#(?:[0-9a-fA-F]{3}){1,2}$/.test(value)) {
        rule.color = value;
        picker.setValue(value);
        this.plugin.saveSettings();
      }
    });
    new import_obsidian.ButtonComponent(ruleSettingDiv).setButtonText("\u25B2").setTooltip("Move Up").setClass("cnb-rule-setting-item-up-button").setDisabled(index == 0).onClick(() => {
      if (index > 0) {
        this.plugin.settings.colorRules.splice(index, 1);
        this.plugin.settings.colorRules.splice(index - 1, 0, rule);
        this.plugin.saveSettings();
        this.display();
      }
    });
    new import_obsidian.ButtonComponent(ruleSettingDiv).setButtonText("\u25BC").setTooltip("Move Down").setClass("cnb-rule-setting-item-down-button").setDisabled(index == this.plugin.settings.colorRules.length - 1).onClick(() => {
      if (index < this.plugin.settings.colorRules.length - 1) {
        this.plugin.settings.colorRules.splice(index, 1);
        this.plugin.settings.colorRules.splice(index + 1, 0, rule);
        this.plugin.saveSettings();
        this.display();
      }
    });
    new import_obsidian.ButtonComponent(ruleSettingDiv).setButtonText("Remove").setClass("cnb-rule-setting-item-remove-button").setCta().onClick(() => {
      this.plugin.settings.colorRules = this.plugin.settings.colorRules.filter((r) => r.id !== rule.id);
      this.plugin.saveSettings();
      this.plugin.removeStyle(rule);
      ruleSettingDiv.remove();
    });
  }
};

// src/main.ts
var ColorfulNoteBordersPlugin = class extends import_obsidian2.Plugin {
  async onload() {
    await this.loadSettings();
    this.addSettingTab(new SettingsTab(this.app, this));
    this.registerEvent(
      this.app.workspace.on("active-leaf-change", this.onActiveLeafChange.bind(this))
    );
    this.registerEvent(
      this.app.metadataCache.on("changed", this.onMetadataChange.bind(this))
    );
    this.registerEvent(
      this.app.vault.on("rename", this.onFileRename.bind(this))
    );
  }
  async onunload() {
    this.settings.colorRules.forEach((rule) => {
      this.removeStyle(rule);
    });
  }
  async removeStyle(rule) {
    const style = this.makeStyleName(rule);
    const styleElement = document.getElementById(style);
    if (styleElement) {
      styleElement.remove();
    }
  }
  async onActiveLeafChange(activeLeaf) {
    this.applyRules();
  }
  async onMetadataChange(file) {
    this.applyRules(file);
  }
  async onFileRename(file) {
    this.applyRules();
  }
  async loadSettings() {
    this.settings = Object.assign({}, DEFAULT_SETTINGS, await this.loadData());
    this.updateStyles();
  }
  async saveSettings() {
    await this.saveData(this.settings);
    this.updateStyles();
    const activeFile = this.app.workspace.getActiveFile();
    if (activeFile) {
      this.onFileRename(activeFile);
    }
  }
  async updateStyles() {
    this.settings.colorRules.forEach((rule) => this.updateStyle(rule));
  }
  async updateStyle(rule) {
    const styleName = this.makeStyleName(rule);
    this.updateCustomCSS(styleName, `
			.${styleName} {
				border: 5px solid ${rule.color} !important;
			}
		`);
  }
  addCustomCSS(cssstylename, css) {
    const styleElement = document.createElement("style");
    styleElement.id = cssstylename;
    styleElement.innerText = css;
    document.head.appendChild(styleElement);
  }
  updateCustomCSS(cssstylename, css) {
    const styleElement = document.getElementById(cssstylename);
    if (styleElement) {
      styleElement.innerText = css;
    } else {
      this.addCustomCSS(cssstylename, css);
    }
  }
  async applyRules(file = null) {
    this.app.workspace.getLeavesOfType("markdown").forEach((value) => {
      if (!(value.view instanceof import_obsidian2.MarkdownView))
        return;
      const activeView = value.view;
      const viewFile = activeView.file;
      if (file && file !== viewFile)
        return;
      const contentView = activeView.containerEl.querySelector(".view-content");
      if (!contentView)
        return;
      this.unhighlightNote(contentView);
      this.settings.colorRules.some((rule) => {
        return this.applyRule(viewFile, rule, contentView);
      });
    });
  }
  applyRule(file, rule, contentView) {
    var _a, _b;
    switch (rule.type) {
      case "folder" /* Folder */: {
        if (this.checkPath(file.path, rule.value)) {
          this.highlightNote(contentView, rule);
          return true;
        }
        break;
      }
      case "frontmatter" /* Frontmatter */: {
        const [key, value] = rule.value.split(":", 2);
        const frontMatterValue = (_b = (_a = this.app.metadataCache.getFileCache(file)) == null ? void 0 : _a.frontmatter) == null ? void 0 : _b[key];
        const normalizedFrontMatterValue = frontMatterValue == null ? void 0 : frontMatterValue.toString().toLowerCase().trim();
        const normalizedValueToHighlight = value == null ? void 0 : value.toString().toLowerCase().trim();
        if (normalizedFrontMatterValue === normalizedValueToHighlight) {
          this.highlightNote(contentView, rule);
          return true;
        }
        break;
      }
    }
    return false;
  }
  highlightNote(element, rule) {
    element.classList.add(this.makeStyleName(rule));
  }
  unhighlightNote(element) {
    this.settings.colorRules.forEach((rule) => {
      element.classList.remove(this.makeStyleName(rule));
    });
  }
  checkPath(currentPath, blacklistedPath) {
    return currentPath.includes(blacklistedPath);
  }
  makeStyleName(rule) {
    return `cnb-${rule.id}-style`;
  }
};

/* nosourcemap */