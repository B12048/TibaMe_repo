using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.Enum
{
    public enum EnumProductCategory
    {
        [Display(Name = "家庭遊戲")]
        FamilyGame = 1,

        [Display(Name = "派對遊戲")]
        PartyGame = 2,

        [Display(Name = "兒童遊戲")]
        KidsGame = 3,

        [Display(Name = "玩家遊戲")]
        StrategyGame = 4,

        [Display(Name = "卡牌遊戲")]
        CardGame = 5,

        [Display(Name = "雙人遊戲")]
        TwoPlayer = 6
    }
}
