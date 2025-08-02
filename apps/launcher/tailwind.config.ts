/** @type {import('tailwindcss').Config} */
module.exports = {
    // NOTE: Update this to include the paths to all of your component files.
    content: ['./index.ts', './src/app/index.tsx', './src/**/*.{js,jsx,ts,tsx}'],
    presets: [require('nativewind/preset')],
    theme: {
        extend: {
            animation: {},
            keyframes: {},
            borderWidth: {
                3: '3px',
                5: '5px',
                6: '6px',
                7: '7px'
            },
            height: {
                17: '4.25rem',
                18: '4.5rem'
            },
            colors: {
                'primary-base': 'var(--primary-base)',
                'text-base': 'var(--text-base)',
                'background-base': 'var(--background-base)',
                'border-base': 'var(--border-base)',
                'card-base': 'var(--card-base)',
                'tint-base': 'var(--tint-base)',
                'icon-base': 'var(--icon-base)'
            },
            stroke: {
                3: '3',
                4: '4'
            },
            screens: {
                sm: '240px',
                md: '420px',
                lg: '1024px',
                xl: '1280px'
            }
        }
    },
    plugins: []
};
