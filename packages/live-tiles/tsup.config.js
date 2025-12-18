import path from 'node:path';
import { fileURLToPath } from 'node:url';
import * as glob from 'glob';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const entries = glob.sync(['./src/**/*.ts{,x}'], { ignore: ['./src/**/*.test.*', './src/**/*.d.ts'] });
console.log(entries, __dirname, path.join(__dirname, './tsconfig.json'));

/**
 * @type {import("tsup").Options}
 */
export default {
    bundle: false,
    clean: true,
    external: ['react'],
    dts: {
        compilerOptions: {
            composite: false // Work-around for dts failing when we're using `composite` mode.
        },
        entry: entries,
        resolve: true
    },
    entry: entries,
    format: ['esm'],
    keepNames: true,
    platform: 'neutral',
    skipNodeModulesBundle: true,
    sourcemap: 'inline',
    splitting: false,
    treeshake: true,
    tsconfig: './tsconfig.json'
};
