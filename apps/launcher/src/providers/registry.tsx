import i18next from '@/utils/i18n';
import type { Theme } from '@react-navigation/native';
import { ThemeProvider } from '@react-navigation/native';
import { MutationCache, QueryCache, QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { ReactNode } from 'react';
import { I18nextProvider } from 'react-i18next';
import { KeyboardProvider } from 'react-native-keyboard-controller';
import { SafeAreaProvider, initialWindowMetrics } from 'react-native-safe-area-context';
import SplashProvider from './splash-provider';

const queryClient = new QueryClient({
    queryCache: new QueryCache({
        onError: (error) => {
            console.error('Error in query:', error);
            // Global error handling here (e.g., Toast notification)
        }
    }),
    mutationCache: new MutationCache({
        onError: (error) => {
            console.error('Error in mutation:', error, (error as any).session);
            // Global error handling for mutations
        }
    })
});

export const ProvidersRegistry = ({ theme, children }: { theme: Theme; children: ReactNode }) => (
    <QueryClientProvider client={queryClient}>
        <KeyboardProvider>
            <ThemeProvider value={theme}>
                <SafeAreaProvider initialMetrics={initialWindowMetrics}>
                    <I18nextProvider i18n={i18next}>
                        <SplashProvider>{children}</SplashProvider>
                    </I18nextProvider>
                </SafeAreaProvider>
            </ThemeProvider>
        </KeyboardProvider>
    </QueryClientProvider>
);
