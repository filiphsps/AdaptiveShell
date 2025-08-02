import 'ts-node/register';

import type { ConfigContext, ExpoConfig } from 'expo/config';

export default ({ config }: ConfigContext): ExpoConfig => ({
    ...config,
    name: 'metro',
    slug: 'metro',
    userInterfaceStyle: 'automatic',
    version: '0.1.0',
    runtimeVersion: '0.1.0',
    orientation: 'portrait',
    icon: './assets/images/icon.png',
    assetBundlePatterns: ['**/*', 'assets/**/*'],
    newArchEnabled: true,
    jsEngine: 'hermes',
    splash: {
        image: './assets/images/splash-icon.png',
        resizeMode: 'contain',
        backgroundColor: '#ffffff'
    },
    ios: {
        bitcode: true,
        supportsTablet: true,
        bundleIdentifier: 'com.filiphsandstrom.metro',
        userInterfaceStyle: 'automatic'
    },
    android: {
        softwareKeyboardLayoutMode: 'pan',
        adaptiveIcon: {
            foregroundImage: './assets/images/adaptive-icon.png',
            backgroundColor: '#ffffff'
        },
        package: 'com.filiphsandstrom.metro',
        userInterfaceStyle: 'automatic',
        edgeToEdgeEnabled: true,
        permissions: []
    },
    web: {
        bundler: 'metro',
        favicon: './assets/images/favicon.png'
    },
    plugins: [
        [
            'expo-asset',
            {
                assets: ['./assets']
            }
        ],
        [
            'expo-router',
            {
                root: './src/app'
            }
        ],
        [
            'expo-dev-client',
            {
                launchMode: 'most-recent'
            }
        ],
        [
            'expo-font',
            {
                fonts: ['./assets/fonts/SpaceMono-Regular.ttf'],
                android: {
                    fonts: [
                        {
                            fontFamily: 'SpaceMono',
                            fontDefinitions: [
                                {
                                    path: './assets/fonts/SpaceMono-Regular.ttf',
                                    weight: 400
                                }
                            ]
                        }
                    ]
                }
            }
        ]
    ],
    experiments: {
        reactCompiler: true,
        tsconfigPaths: true,
        typedRoutes: true
    },
    extra: {
        router: {
            origin: false,
            root: './src/app/_layout.tsx'
        }
    }
});
