import _ from 'lodash';
import { useCallback, useEffect, useState } from 'react';
import { Appearance } from 'react-native';

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
        []
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
