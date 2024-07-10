using System.Text;

namespace MockPOS.Main.Local.Helper
{
    public static class ControlCharacterHelper
    {
        private static readonly Dictionary<char, string> ControlCharacterMap = new Dictionary<char, string>
        {
            { '\u0000', "<NUL>" },  // Null
            { '\u0001', "<SOH>" },  // Start of Heading
            { '\u0002', "<STX>" },  // Start of Text
            { '\u0003', "<ETX>" },  // End of Text
            { '\u0004', "<EOT>" },  // End of Transmission
            { '\u0005', "<ENQ>" },  // Enquiry
            { '\u0006', "<ACK>" },  // Acknowledge
            { '\u0007', "<BEL>" },  // Bell
            { '\u0008', "<BS>" },   // Backspace
            { '\u0009', "<HT>" },   // Horizontal Tab
            { '\n', "<LF>" },       // Line Feed
            { '\u000B', "<VT>" },   // Vertical Tab
            { '\u000C', "<FF>" },   // Form Feed
            { '\r', "<CR>" },       // Carriage Return
            { '\u000E', "<SO>" },   // Shift Out
            { '\u000F', "<SI>" },   // Shift In
            { '\u0010', "<DLE>" },  // Data Link Escape
            { '\u0011', "<DC1>" },  // Device Control 1 (XON)
            { '\u0012', "<DC2>" },  // Device Control 2
            { '\u0013', "<DC3>" },  // Device Control 3 (XOFF)
            { '\u0014', "<DC4>" },  // Device Control 4
            { '\u0015', "<NAK>" },  // Negative Acknowledge
            { '\u0016', "<SYN>" },  // Synchronous Idle
            { '\u0017', "<ETB>" },  // End of Transmission Block
            { '\u0018', "<CAN>" },  // Cancel
            { '\u0019', "<EM>" },   // End of Medium
            { '\u001A', "<SUB>" },  // Substitute
            { '\u001B', "<ESC>" },  // Escape
            { '\u001C', "<FS>" },   // File Separator
            { '\u001D', "<GS>" },   // Group Separator
            { '\u001E', "<RS>" },   // Record Separator
            { '\u001F', "<US>" },   // Unit Separator
            { '\u007F', "<DEL>" },  // Delete
        };

        public static string ConvertToReadableString(string text)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in text)
            {
                if (ControlCharacterMap.ContainsKey(c))
                {
                    result.Append(ControlCharacterMap[c]);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
    }
}
