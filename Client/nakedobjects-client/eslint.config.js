const eslint = require("@eslint/js");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");

module.exports = tseslint.config(
  {
    files: ["**/*.ts"],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...tseslint.configs.stylistic,
      ...angular.configs.tsRecommended,
    ],
    processor: angular.processInlineTemplates,
    rules: {
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "nof",
          style: "camelCase",
        },
      ],
       "@angular-eslint/prefer-standalone": [
        "off"
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "nof",
          style: "kebab-case",
        },
      ],
     
      "no-unused-vars": "off",
      "no-empty-function": "off",
      "@typescript-eslint/no-non-null-assertion": "off",
      "@typescript-eslint/no-unused-vars": [
        "error", {
          "argsIgnorePattern": "^_",
          "varsIgnorePattern": "^_",
          "caughtErrorsIgnorePattern": "^_"
        }
      ],
      "@typescript-eslint/no-explicit-any": "error",
      "@typescript-eslint/no-empty-function": "error",
      "@typescript-eslint/no-empty-object-type": "off",
    },
  },
  {
    files: ["**/*.html"],
    extends: [
      ...angular.configs.templateRecommended,
      ...angular.configs.templateAccessibility,
    ],
    rules: { "@angular-eslint/template/click-events-have-key-events" :  [
      "off"
    ],
    "@angular-eslint/template/interactive-supports-focus" : [
      "off"
    ],},
  }
);
