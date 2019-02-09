#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("t/16kufbbQbCcEY90as38HH2YsEBVzmLCBgPClwUKQ2LCAE5iwgNOT+QRSRxvuSFktX6fpL7f9t+OUbIKWhnbSlqbHt9YG9gamh9YGZnKXknSa/+TkR2AVc5Fg8KXBQqDRE5H74StJpLLRsjzgYUv0SVV2rBQokeQNF/ljodbKh+ncAkCwoICQiqiwhQrgwAdR5JXxgXfdq+gioyTqrcZmdtKWpmZ21gfWBmZ3opZm8pfHpsDQ8aC1xaOBo5GA8KXA0DGgNIeXlzOYsIfzkHDwpcFAYICPYNDQoLCDo/UzlrOAI5AA8KXA0PGgtcWjgafWBvYGpofWwpa3ApaGdwKXloe315ZWwpW2ZmfSlKSDkXHgQ5Pzk9Oylmbyl9YWwpfWFsZyloeXllYGpoDuV0MIqCWinaMc24tpNGA2L2IvUMCQqLCAYJOYsIAwuLCAgJ7ZigAGtlbCl6fWhnbWh7bSl9bHtkeilofn4naHl5ZWwnamZkJmh5eWVsamgEDwAjj0GP/gQICAwMCQqLCAgJVRaY0hdOWeIM5FdwjSTiP6teRVzlTHcWRWJZn0iAzX1rAhmKSI46g4g8Ozg9OTo/Ux4EOjw5OzkwOzg9OTkYDwpcDQMaA0h5eWVsKUBnaic4NC9uKYM6Y/4Ei8bX4qom8FpjUm2JHSLZYE6dfwD3/WKEJ0mv/k5Edh85HQ8KXA0KGgRIeXllbClbZmZ9eWVsKUpse31gb2BqaH1gZmcpSHzJajp+/jMOJV/i0wYoB9OzehBGvIZ6iGnPElIAJpu78U1B+Wkxlxz8bTwqHEIcUBS6nf7/lZfGWbPIUVnQP3bIjlzQrpCwO0vy0dx4l3eoW4sICQ8AI49Bj/5qbQwIOYj7OSMPe2hqfWBqbCl6fWh9bGRsZ316JzkWjIqMEpA0Tj77oJJJhyXduJkb0bwzpP0GBwmbArgoHyd93DUE0msfdkihkfDYw2+VLWIY2aqy7RIjyhYPOQYPClwUGggI9g0MOQoICPY5FC3r4ti+edYGTOguw/hkceTuvB4eZWwpQGdqJzgvOS0PClwNAhoUSHkGlDT6IkAhE8H3x7ywB9BXFd/CNFtsZWBoZ2psKWZnKX1hYHopamx7boYBvSn+wqUlKWZ5vzYIOYW+SsYlKWpse31gb2BqaH1sKXlmZWBqcIIQgNfwQmX8DqIrOQvhETfxWQDanJdzBa1OglLdHz46ws0GRMcdYNgBIg8IDAwOCwgfF2F9fXl6MyYmfmBvYGpofWBmZylIfH1hZntgfXA4cCloenp8ZGx6KWhqamx5fWhnamwvOS0PClwNAhoUSHl5ZWwpSmx7fcAQe/xUB9x2VpL7LAqzXIZEVAT4JjmIyg8BIg8IDAwOCws5iL8TiLojj0GP/gQICAwMCTlrOAI5AA8KXH1hZntgfXA4HzkdDwpcDQoaBEh5DwpcFAcNHw0dItlgTp1/APf9YoQ5iw2yOYsKqqkKCwgLCwgLOQQPALg5UeVTDTuFYbqGFNdsevZuV2y1oqp4m05aXMimJki68fLqecTvqkUpSkg5iwgrOQQPACOPQY/+BAgICKHVdys8wyzc0AbfYt2rLSoY/qilWaOD3NPt9dkADj65fHwo");
        private static int[] order = new int[] { 50,33,48,45,31,59,27,52,43,48,28,43,39,27,41,51,41,42,36,47,51,25,52,48,56,32,39,31,57,55,51,58,34,46,49,48,42,55,53,45,52,53,49,44,53,49,48,53,52,57,53,55,52,59,55,59,57,57,58,59,60 };
        private static int key = 9;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
