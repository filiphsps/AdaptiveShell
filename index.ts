import '@bacons/text-decoder/install';
import { configureReanimatedLogger, ReanimatedLogLevel } from 'react-native-reanimated';

configureReanimatedLogger({
    level: ReanimatedLogLevel.error,
    strict: true
});

import 'expo-router/entry';
