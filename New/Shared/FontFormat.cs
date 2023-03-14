namespace New.Shared
{
  public sealed class FontFormat
  {
    public static readonly Dictionary<char, FontFormat> VALUES = new();

    public static readonly FontFormat BLACK = new(0, '0');
    public static readonly FontFormat DARKBLUE = new(0xff0000aa, '1');
    public static readonly FontFormat DARKGREEN = new(0xff00aa00, '2');
    public static readonly FontFormat DARKCYAN = new(0xff00aaaa, '3');
    public static readonly FontFormat DARKRED = new(0xffaa0000, '4');
    public static readonly FontFormat DARKPURPLE = new(0xffaa00aa, '5');
    public static readonly FontFormat GOLD = new(0xffffaa00, '6');
    public static readonly FontFormat GRAY = new(0xffaaaaaa, '7');
    public static readonly FontFormat DARKGRAY = new(0xff555555, '8');
    public static readonly FontFormat BLUE = new(0xff5555ff, '9');
    public static readonly FontFormat GREEN = new(0xff55ff55, 'a');
    public static readonly FontFormat CYAN = new(0xff55ffff, 'b');
    public static readonly FontFormat RED = new(0xffff5555, 'c');
    public static readonly FontFormat PURPLE = new(0xffff55ff, 'd');
    public static readonly FontFormat YELLOW = new(0xffffff55, 'e');
    public static readonly FontFormat WHITE = new(0xffffffff, 'f');
    public static readonly FontFormat RESET = new(0, 'r');
    private readonly uint _code;

    public readonly uint Color;

    private FontFormat(uint color, char code)
    {
      Color = color;
      _code = code;
      VALUES[code] = this;
    }

    public override string ToString()
    {
      return $"\u00a7{_code}";
    }
  }
}