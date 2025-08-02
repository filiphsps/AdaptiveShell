import type { Theme } from '@react-navigation/native';
import { DarkTheme, DefaultTheme } from '@react-navigation/native';
import { vars } from 'nativewind';

export type ExtendedTheme = Theme & {
    colors: Theme['colors'] & {
        icon: string;
        tint: string;
    };
};

export const LIGHT_THEME: ExtendedTheme = {
    dark: false,
    colors: {
        primary: '#11181C',
        text: '#11181C',
        background: '#fcfcfc',
        card: 'rgb(255, 255, 255)',
        tint: '#0a7ea4',
        icon: 'rgb(60, 60, 60)',
        border: 'rgb(216, 216, 216)',
        notification: 'rgb(255, 59, 48)'
    },
    fonts: {
        ...(DefaultTheme.fonts as Theme['fonts'])
    }
};
export const DARK_THEME: ExtendedTheme = {
    dark: true,
    colors: {
        primary: '#ECEDEE',
        text: '#ECEDEE',
        background: '#000000',
        card: 'rgb(18, 18, 18)',
        tint: '#ffffff',
        icon: 'rgb(130, 130, 130)',
        border: 'rgb(39, 39, 41)',
        notification: 'rgb(255, 69, 58)'
    },
    fonts: {
        ...(DarkTheme.fonts as Theme['fonts'])
    }
};

export const TAILWIND_THEME = {
    light: vars({
        '--primary-base': LIGHT_THEME.colors.primary,
        '--text-base': LIGHT_THEME.colors.text,
        '--background-base': LIGHT_THEME.colors.background,
        '--border-base': LIGHT_THEME.colors.border,
        '--card-base': LIGHT_THEME.colors.card,
        '--tint-base': LIGHT_THEME.colors.tint,
        '--icon-base': LIGHT_THEME.colors.tint
    }),
    dark: vars({
        '--primary-base': DARK_THEME.colors.primary,
        '--text-base': DARK_THEME.colors.text,
        '--background-base': DARK_THEME.colors.background,
        '--border-base': DARK_THEME.colors.border,
        '--card-base': DARK_THEME.colors.card,
        '--tint-base': DARK_THEME.colors.tint,
        '--icon-base': DARK_THEME.colors.icon
    })
};
