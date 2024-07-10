namespace PayPro.Contracts.Models.POS
{
    public static class PaymentPacketUtils
    {
        public static string CalculateChecksum(string packet)
        {
            int checksum = 0;
            foreach (char c in packet)
            {
                if (c != AsciiCC.STX && c != AsciiCC.ETX)
                {
                    checksum ^= c;
                }
            }
            return checksum.ToString("X2");
        }
    }
}
