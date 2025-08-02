import '@/styles/global.css';

import { useColorScheme } from '@/hooks/use-color-scheme';
import { useTheme } from '@/hooks/use-theme';
import { ProvidersRegistry } from '@/providers/registry';
import { TAILWIND_THEME } from '@/styles/theme';
import { Stack } from 'expo-router';
import { StatusBar } from 'expo-status-bar';
import { GestureHandlerRootView } from 'react-native-gesture-handler';

export default function Layout() {
    const { theme } = useTheme();

    const { colorScheme } = useColorScheme();
    const activeTheme = colorScheme === 'dark' ? 'dark' : 'light';

    return (
        <GestureHandlerRootView className="flex-1 bg-background-base" style={TAILWIND_THEME[activeTheme]}>
            <ProvidersRegistry theme={theme}>
                <StatusBar style="auto" />

                <Stack screenOptions={{ headerShown: false }}>
                    <Stack.Screen name="(root)" />
                </Stack>
            </ProvidersRegistry>
        </GestureHandlerRootView>
    );
}
