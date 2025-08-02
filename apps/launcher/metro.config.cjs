const { getDefaultConfig } = require('expo/metro-config');
const { withNativeWind } = require('nativewind/metro');

module.exports = (() => {
    /** @type {import('expo/metro-config').MetroConfig} */
    const config = getDefaultConfig(__dirname); // eslint-disable-line no-undef
    const { transformer, resolver } = config;

    config.transformer = {
        ...transformer,
        babelTransformerPath: require.resolve('react-native-svg-transformer/expo')
    };
    config.resolver = {
        ...resolver,
        assetExts: [...resolver.assetExts.filter((ext) => ext !== 'svg'), 'wasm'],
        sourceExts: [...resolver.sourceExts, 'svg', 'sql'],
        unstable_enablePackageExports: false
    };

    return withNativeWind(config, {
        input: './src/styles/global.css',
        configPath: './tailwind.config.ts'
    });
})();
