import { useColorScheme } from '@/hooks/use-color-scheme';
import { cn } from '@/utils/cn';
import { useFonts } from 'expo-font';
import * as SplashScreen from 'expo-splash-screen';
import type { ReactNode } from 'react';
import { useEffect, useState } from 'react';
import { Text, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

SplashScreen.preventAutoHideAsync();
SplashScreen.setOptions({
    duration: 500,
    fade: true
});

export default function SplashProvider({ children }: { children: ReactNode }) {
    const { colorScheme } = useColorScheme();
    const [fontsLoaded] = useFonts({
        SpaceMono: require('../../assets/fonts/SpaceMono-Regular.ttf')
    });

    const [showSplash, setShowSplash] = useState(true);

    useEffect(() => {
        if (!colorScheme) return;
        SplashScreen.hideAsync();

        if (!fontsLoaded || !showSplash) return;
        setShowSplash(false);
    }, [colorScheme, fontsLoaded, showSplash]);

    let loadingState = '';
    if (!fontsLoaded) {
        loadingState = 'fonts';
    }

    return (
        <>
            <View
                className={cn(
                    'absolute inset-0 z-[1000] flex-1 bg-background-base opacity-100 transition-opacity duration-500',
                    !showSplash && 'pointer-events-none opacity-0'
                )}
            >
                <SafeAreaView className="flex-1 select-none items-center justify-center gap-6">
                    <Text className="text-center">
                        <>{loadingState ? ` ${loadingState}` : ''}...</>
                    </Text>
                </SafeAreaView>
            </View>

            {children}
        </>
    );
}
