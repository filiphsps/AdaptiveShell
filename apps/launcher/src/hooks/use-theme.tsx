import { useColorScheme } from '@/hooks/use-color-scheme';
import { DARK_THEME, LIGHT_THEME } from '@/styles/theme';

export function useTheme() {
    const { colorScheme } = useColorScheme();

    const activeTheme = colorScheme === 'dark' ? 'dark' : 'light';
    const theme = activeTheme === 'dark' ? DARK_THEME : LIGHT_THEME;

    return { theme };
}
