import { Size } from '../utils/size';

export class TileDensity {
    public static desktop(): TileDensity {
        return new TileDensity(new Size(48, 48), new Size(100, 100), new Size(204, 100), new Size(204, 204));
    }

    public static tablet(): TileDensity {
        return new TileDensity(new Size(60, 60), new Size(125, 125), new Size(255, 125), new Size(255, 255));
    }

    public static mobile(customDensity: number): TileDensity {
        return new TileDensity(
            new Size(48 * customDensity, 48 * customDensity),
            new Size(100 * customDensity, 100 * customDensity),
            new Size(204 * customDensity, 100 * customDensity),
            new Size(204 * customDensity, 204 * customDensity)
        );
    }

    public readonly small: Size;
    public readonly medium: Size;
    public readonly wide: Size;
    public readonly large: Size;

    private constructor(small: Size, medium: Size, wide: Size, large: Size) {
        this.small = small;
        this.medium = medium;
        this.wide = wide;
        this.large = large;
    }
}
