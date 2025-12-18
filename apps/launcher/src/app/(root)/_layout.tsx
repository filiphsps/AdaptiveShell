import '~/styles/global.css';

import { Stack } from 'expo-router';

export default function RootLayout() {
    return (
        <Stack screenOptions={{ headerShown: false }}>
            <Stack.Screen name="(startscreen)" options={{ title: 'Tabs' }} />
        </Stack>
    );
}
