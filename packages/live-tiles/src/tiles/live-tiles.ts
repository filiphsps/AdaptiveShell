import type { Optional } from '../@types';

export enum LiveTileBranding {
    None = 0,
    Logo,
    Name,
    NameAndLogo
}

export enum AdaptiveTextStacking {
    Default = 0,
    Top,
    Center,
    Bottom
}

export enum AdaptiveImageAlign {
    Default = 0,
    Stretch,
    Left,
    Center,
    Right
}

export enum AdaptiveTextStyle {
    Default = 0,
    Caption,
    CaptionSubtle,
    Body,
    BodySubtle,
    Base,
    BaseSubtle,
    Subtitle,
    SubtitleSubtle,
    Title,
    TitleSubtle,
    TitleNumeral,
    Subheader,
    SubheaderSubtle,
    SubheaderNumeral,
    Header,
    HeaderSubtle,
    HeaderNumeral
}
export enum AdaptiveTextAlign {
    Default = 0,
    Auto,
    Left,
    Center,
    Right
}

export abstract class AdaptiveContent {}
export class AdaptiveImage extends AdaptiveContent {
    public constructor(
        public readonly source: string,
        public readonly align: AdaptiveImageAlign = AdaptiveImageAlign.Default
    ) {
        super();
    }
}
export class AdaptiveText extends AdaptiveContent {
    public constructor(
        public readonly text: string,
        public readonly wrap: boolean = false,
        public readonly style: AdaptiveTextStyle = AdaptiveTextStyle.Default,
        public readonly align: AdaptiveTextAlign = AdaptiveTextAlign.Default
    ) {
        super();
    }
}

export abstract class LiveTileImage {
    public constructor(
        public readonly source: string,
        public readonly overlay: number = 20
    ) {}
}
export class LiveTileBadgeImage extends LiveTileImage {}
export class LiveTilePeekImage extends LiveTileImage {}
export class LiveTileBackgroundImage extends LiveTileImage {}

export abstract class LiveTileContent {}
export class LiveTileContentAdaptive extends LiveTileContent {
    public constructor(
        public readonly peekImage: Optional<LiveTilePeekImage>,
        public readonly backgroundImage: Optional<LiveTileBackgroundImage>,
        public readonly textStacking: AdaptiveTextStacking = AdaptiveTextStacking.Default,
        public readonly children: AdaptiveContent[] = []
    ) {
        super();
    }
}

/**
 * A tile that displays a single line of text with an optional badge.
 *
 * ![screenshot](https://learn.microsoft.com/en-us/previous-versions/windows/apps/images/hh761491.tilesquare150x150iconwithbadge(en-us,win.10).jpg)
 *
 *{@link https://learn.microsoft.com/en-us/previous-versions/windows/apps/hh761491(v=win.10)#tilesquare150x150iconwithbadge}
 * @extends {LiveTileContent}
 */
export class LiveTileContentIconWithBadge extends LiveTileContent {
    public constructor(
        public readonly badgeImage: Optional<LiveTileBadgeImage>,
        public readonly text: string = ''
    ) {
        super();
    }
}

/**
 * A tile that displays two rows of text either side by side or one above the other depending on the device/platform.
 * They were typically used for display the day and month the calendar app tile.
 *
 * ![screenshot](https://learn.microsoft.com/en-us/previous-versions/windows/apps/images/hh761491.tilesquareblock(en-us,win.10).png)
 *
 * {@link https://learn.microsoft.com/en-us/previous-versions/windows/apps/hh761491(v=win.10)#tilesquareblocktilesquare150x150block}
 * @extends {LiveTileContent}
 */
export class LiveTileContentTileSquareBlock extends LiveTileContent {
    public constructor(
        public readonly blockText: Optional<string>,
        public readonly besideBlockText: Optional<string>,
        public readonly text: string = ''
    ) {
        super();
    }
}

export class LiveTileBinding {
    public constructor(
        public displayName: Optional<string>,
        public branding: Optional<LiveTileBranding>,
        public content: LiveTileContent
    ) {}
}
