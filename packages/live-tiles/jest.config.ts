import type { Config } from 'jest';

const jestConfig: Config = {
    globals: {
        __DEV__: true
    },
    rootDir: '.',
    preset: 'react-native',
    collectCoverage: true,
    coverageProvider: 'v8',
    coverageReporters: ['json', 'text', 'text-summary'],
    json: true,
    collectCoverageFrom: ['./src/**/*.*', '!**/node_modules/**'],
    testEnvironment: 'node',
    transform: {
        '^.+\\.tsx?$': [
            'ts-jest',
            {
                tsconfig: 'tsconfig.test.json'
            }
        ],
        '^.+\\.jsx?$': ['babel-jest', { configFile: './babel.config.cjs' }]
    },
    moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx', 'json', 'node'],
    testPathIgnorePatterns: ['dist/', 'node_modules/'],
    transformIgnorePatterns: ['/node_modules/(?!(@react-native|react-native|react-native-gesture-handler)/).*/'],
    testRegex: '(/__tests__/.*|(\\.|/)(test|spec))\\.[mc]?[jt]sx?$',
    clearMocks: true,
    useStderr: true
};

export default jestConfig;
