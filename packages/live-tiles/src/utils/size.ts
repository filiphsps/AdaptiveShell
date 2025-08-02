/**
 * A Size, essentially a 2D Vector.
 */
export class Size {
    /**
     * Returns a Size with 0 as it's width and height.
     */
    public static get ZERO(): Size {
        return new Size(0, 0);
    }

    /**
     * Create a new `Size` instance.
     * @param {number} width - The width.
     * @param {number} height - The height.
     * @example
     * ```typescript
     * const size = new Size(10, 20);
     * ```
     */
    public constructor(
        protected width: number = 0,
        protected height: number = 0
    ) {}

    /**
     * Creates a new Size instance from an object with width and height properties.
     * @param obj - The object containing width and height properties.
     * @returns {Size} A new Size instance.
     */
    public static fromObject({ width, height }: { width: number; height: number }): Size {
        return new Size(width, height);
    }

    /**
     * Set the width.
     * @param {number} width - The width.
     * @example
     * ```typescript
     * size.setX(10);
     * ```
     */
    public setWidth(width: number): void {
        this.width = width;
    }

    /**
     * Set the height.
     * @param {number} height - The height.
     * @example
     * ```typescript
     * size.setZ(10);
     * ```
     */
    public setHeight(height: number): void {
        this.height = height;
    }

    /**
     * Get the width.
     * @returns {number} The width.
     */
    public getWidth(): number {
        return this.width;
    }

    /**
     * Get the height.
     * @returns {number} The height.
     */
    public getHeight(): number {
        return this.height;
    }
}
