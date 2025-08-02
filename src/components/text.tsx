import { cn } from '@/utils/cn';
import { Text as RNText, type TextProps as RNTextProps } from 'react-native';

export type TextProps = RNTextProps & {
    type?: 'default' | 'title' | 'defaultSemiBold' | 'subtitle' | 'link';
};

export function Text({ className, type = 'default', ...rest }: TextProps) {
    return (
        <RNText
            className={cn(
                'text-base text-text-base',
                type === 'defaultSemiBold' && 'font-semibold',

                type === 'title' && 'text-4xl font-bold leading-[1.1]',
                type === 'subtitle' && 'text-xl font-medium',
                type === 'link' && 'text-cyan-600 underline underline-offset-2',
                className
            )}
            {...rest}
        />
    );
}
