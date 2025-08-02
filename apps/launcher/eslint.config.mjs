import { FlatCompat } from '@eslint/eslintrc';
import js from '@eslint/js';
import typescriptEslint from '@typescript-eslint/eslint-plugin';
import tsParser from '@typescript-eslint/parser';
import expoRecommendedConfig from 'eslint-config-expo/flat.js';
// import prettier from 'eslint-plugin-prettier';
// import prettierRecommendedConfig from 'eslint-plugin-prettier/recommended';
import react from 'eslint-plugin-react';
import reactCompiler from 'eslint-plugin-react-compiler';
import unusedImports from 'eslint-plugin-unused-imports';
import { defineConfig, globalIgnores } from 'eslint/config';
import globals from 'globals';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const compat = new FlatCompat({
    baseDirectory: __dirname,
    recommendedConfig: js.configs.recommended,
    allConfig: js.configs.all
});

export default defineConfig([
    expoRecommendedConfig,
    // prettierRecommendedConfig,
    globalIgnores([
        '**/node_modules/',
        '**/dist/',
        '**/coverage/',
        '**/dist/',
        '**/.next/',
        '**/.now/',
        '**/public/',
        '**/.vitest/',
        '**/*.json',
        '**/*.lock',
        '**/*.env',
        '**/*.log',
        '**/*.bak',
        '**/*.d.ts',
        '**/vitest.setup.ts',
        '**/eslint.config.*',
        '**/tailwind.config.*',
        'src/codegen/',
        'src/utils/i18n.ts'
    ]),
    {
        extends: compat.extends('plugin:react/recommended'),

        files: ['**/*.ts', '**/*.tsx', 'scripts/**/*.ts'],

        plugins: {
            // prettier,
            'react-compiler': reactCompiler,
            '@typescript-eslint': typescriptEslint,
            'unused-imports': unusedImports,
            react
        },

        languageOptions: {
            globals: {
                ...globals.node
            },

            parser: tsParser,
            ecmaVersion: 'latest',
            sourceType: 'module',

            parserOptions: {
                ecmaFeatures: {
                    jsx: true
                },

                project: ['./tsconfig.json', './tsconfig.test.json']
            }
        },

        settings: {
            react: {
                version: 'detect'
            }
        },

        rules: {
            '@next/next/no-html-link-for-pages': 'off',

            '@typescript-eslint/consistent-type-exports': [
                'error',
                {
                    fixMixedExportsWithInlineTypeSpecifier: false
                }
            ],

            '@typescript-eslint/consistent-type-imports': [
                'error',
                {
                    fixStyle: 'separate-type-imports',
                    prefer: 'type-imports'
                }
            ],

            '@typescript-eslint/no-require-imports': 'error',
            '@typescript-eslint/no-unnecessary-condition': 'warn',

            'brace-style': [
                'error',
                '1tbs',
                {
                    allowSingleLine: true
                }
            ],

            'consistent-return': 'error',
            'import/first': 'off',
            'import/order': 'off',
            indent: 'off',

            'no-console': [
                'warn',
                {
                    allow: ['debug', 'warn', 'error']
                }
            ],

            'no-mixed-operators': 'off',
            'no-unused-vars': 'off',
            'no-useless-constructor': 'off',
            //'prettier/prettier': 'error',
            'react/jsx-uses-react': 'off',
            'react/no-children-prop': 'off',
            'react/no-find-dom-node': 'off',
            'react/no-string-refs': 'off',
            'react/prop-types': 'off',
            'react/react-in-jsx-scope': 'off',
            'react/display-name': 'off',
            semi: ['error', 'always'],
            'sort-imports': 'off',

            'standard/computed-property-even-spacing': 'off',
            'unused-imports/no-unused-imports': 'warn',

            'unused-imports/no-unused-vars': [
                'warn',
                {
                    vars: 'all',
                    varsIgnorePattern: '^_',
                    args: 'after-used',
                    argsIgnorePattern: '^_'
                }
            ]
        }
    }
]);
