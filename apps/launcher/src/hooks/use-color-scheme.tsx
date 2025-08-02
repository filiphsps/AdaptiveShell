import { useEffect, useState, useCallback } from 'react';
import { Appearance } from 'react-native';
import _ from 'lodash';

export function useColorScheme(delay = 250) {
    const [colorScheme, setColorScheme] = useState(Appearance.getColorScheme());

    // eslint-disable-next-line react-hooks/exhaustive-deps
    const onColorSchemeChange = useCallback(
        _.throttle(
            (theme: any) => {
                setColorScheme(theme.colorScheme);
            },
            delay,
            { leading: false }
        ),
        [delay, setColorScheme]
    );

    useEffect(() => {
        const subscription = Appearance.addChangeListener(onColorSchemeChange);
        return () => {
            onColorSchemeChange.cancel();
            subscription.remove();
        };
    }, [onColorSchemeChange]);

    return { colorScheme };
}
