module.exports = {
    extends: ['@commitlint/config-conventional'],
    rules: {
        'subject-case': [2, 'always', 'sentence-case'],
        'subject-full-stop': [2, 'always', '.']
    }
};
