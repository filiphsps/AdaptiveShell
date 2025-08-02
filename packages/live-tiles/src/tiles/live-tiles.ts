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

export class LiveTileBinding {
    public constructor(
        public displayName: Optional<string>,
        public branding: Optional<LiveTileBranding>,
        public content: LiveTileContent
    ) {}
}
