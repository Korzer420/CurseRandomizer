namespace CurseRandomizer;

public enum RequestMethod
{
    /// <summary>
    /// The mod will add additional curse items.
    /// </summary>
    Add,

    /// <summary>
    /// The mod will replace "junk" items which are not on the blacklist with curse items. If no items are available anymore, the remaining items will be added seperately.
    /// </summary>
    Replace,

    /// <summary>
    /// The mod will replace "junk" items which are not on the blacklist with curse items.
    /// </summary>
    ForceReplace
}
