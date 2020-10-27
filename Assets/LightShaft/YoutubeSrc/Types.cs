namespace YoutubeLight
{
    public enum AudioType
    {
        Aac,
        Mp3,
        Opus,
        Vorbis,
        Unknown
    }
}

namespace YoutubeLight
{
    public enum VideoType
    {

        Mobile_3gp,
        Flv,
        Hls,
        Mp4,
        WebM,
        /// <summary>
        /// The video type is unknown. This can occur if YoutubeExtractor is not up-to-date.
        /// </summary>
        Unknown
    }
}

namespace YoutubeLight
{
    public enum AdaptiveType
    {
        None,
        Audio,
        Video,
        Audio_Video
    }
}
