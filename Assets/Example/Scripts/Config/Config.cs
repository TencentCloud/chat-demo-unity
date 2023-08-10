using System.Collections.Generic;

namespace Com.Tencent.IM.Unity.UIKit.Example
{
  public class Config
  {
    // 初始化时使用
    public static string sdkappid = "";
    public static string smsLoginHttpBase =
      "https://demos.trtc.tencent-cloud.com/prod";
    public static string captchaUrl =
        "/captcha.html";

    public static string communityId = "";
    public static string channelId = "";
    public static string groupId = "";
    

    public static List<StickerPackage> stickers = new List<StickerPackage> {
      new StickerPackage {
        name = "4350",
        baseUrl = "custom_sticker_resource/4350",
        menuItem = new StickerItem {
          name = "menu@2x",
          index = 0,
        },
        stickerList = new List<StickerItem> {
          new StickerItem {
          name = "menu@2x",
          index = 0
        },
        new StickerItem {
          name = "yz01@2x",
          index = 1
        },
        new StickerItem {
          name = "yz02@2x",
          index = 2
        },
        new StickerItem {
          name = "yz03@2x",
          index = 3
        },
        new StickerItem {
          name = "yz04@2x",
          index = 4
        },
        new StickerItem {
          name = "yz05@2x",
          index = 5
        },
        new StickerItem {
          name = "yz06@2x",
          index = 6
        },
        new StickerItem {
          name = "yz07@2x",
          index = 7
        },
        new StickerItem {
          name = "yz08@2x",
          index = 8
        },
        new StickerItem {
          name = "yz09@2x",
          index = 9
        },
        new StickerItem {
          name = "yz10@2x",
          index = 10
        },
        new StickerItem {
          name = "yz11@2x",
          index = 11
        },
        new StickerItem {
          name = "yz12@2x",
          index = 12
        },
        new StickerItem {
          name = "yz13@2x",
          index = 13
        },
        new StickerItem {
          name = "yz14@2x",
          index = 14
        },
        new StickerItem {
          name = "yz15@2x",
          index = 15
        },
        new StickerItem {
          name = "yz16@2x",
          index = 16
        },
        new StickerItem {
          name = "yz17@2x",
          index = 17
        }
        }
      }
    };
  }
}