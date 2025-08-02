import { describe, expect, it } from '@jest/globals';

import { cn } from '@/utils/cn';

describe('utils', () => {
    describe('cn', () => {
        it('should return `undefined` when given no class name', () => {
            const result = cn();
            expect(result).toBeUndefined();
        });

        it('should merge tailwind classes', () => {
            const result = cn('text-red-500', 'text-green-500');
            expect(result).toBe('text-green-500');
        });

        it('should keep unknown classes', () => {
            const result = cn('text-red-500', 'unknown-class');
            expect(result).toBe('text-red-500 unknown-class');
        });
    });
});
