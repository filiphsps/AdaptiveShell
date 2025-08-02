import { describe, expect, it } from '@jest/globals';

import { Size } from './size';

describe('Size', () => {
    it('should retrieve values correctly', () => {
        const size = new Size(2.75, 0);

        expect(size.getWidth()).toBe(2.75);
        expect(size.getHeight()).toBe(0);
    });

    it('should convert an object to a size properly', () => {
        const size = Size.fromObject({ width: 250, height: 100 });

        expect(size.getWidth()).toBe(250);
        expect(size.getHeight()).toBe(100);
    });

    it('should get a zero width/height size from Size.ZERO', () => {
        const size = Size.ZERO;

        expect(size.getWidth()).toBe(0);
        expect(size.getHeight()).toBe(0);
    });

    it('should set and get the width coordinate correctly', () => {
        let size = new Size();

        size.setWidth(10);
        expect(size.getWidth()).toBe(10);
    });

    it('should set and get the height coordinate correctly', () => {
        let size = new Size();

        size.setHeight(10);
        expect(size.getHeight()).toBe(10);
    });
});
