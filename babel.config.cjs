module.exports = function (api) {
    api.cache(true);
    return {
        presets: [
            [
                'babel-preset-expo',
                {
                    jsxImportSource: 'nativewind',
                    'react-compiler': {}
                }
            ],
            'nativewind/babel'
        ],
        plugins: ['react-native-reanimated/plugin']
    };
};
