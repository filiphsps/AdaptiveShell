import '~/styles/global.css';

import { Stack } from 'expo-router';

export default function StartScreenLayout() {
    return (
        <Stack screenOptions={{ headerShown: false }}>
            <Stack.Screen name="index" options={{ title: 'Start Screen' }} />
        </Stack>
    );
}
