using System.Collections.Generic;

namespace Com.Tencent.IM.Unity.UIKit
{
  public class StickerPackage
  {
    public string name { get; set; }
    public string baseUrl { get; set; }
    // Emoji stickers go with the text
    public bool? isEmoji { get; set; }
    // Is unicode sticker
    public bool? isUnicode { get; set; }
    public List<StickerItem> stickerList { get; set; }
    public StickerItem menuItem { get; set; }
  }

  public class StickerItem
  {
    public int? unicode { get; set; }
    public string name { get; set; }
    public int index { get; set; }
    public string url { get; set; }
  }
}