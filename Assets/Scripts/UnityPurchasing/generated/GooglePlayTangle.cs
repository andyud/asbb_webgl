// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DSJbtJ56vvgUqldGlQtlfG1e6zCHBAoFNYcEDweHBAQFx5fGqfywg2I/2A4AKUfJZYBoyQxE3X9+zg4uiHiCJT38oU94ldY6mTELrQwbffU1VgwsaNmAT2EJBh5RQ+PhBjwc9ev3OtMEkrr1/LvrD/umINa7+lY7NYcEJzUIAwwvg02D8ggEBAQABQaaExeBMdmURSplJ+fq78/NfC+4AaBfO4w4+Yj7pdHHUSUWYR5K1tv+jiVUVyhnb4iOK9XO5nHMp9X3ajLM1lH0vzwtrkzsxL3Lf1uUXg4v85/SsjZ0FQ8HY9wniL/jJoyY2OF1CJsTxZyJ8YlXOaW2pxC8xTc3MVVdtNVr0IB+ZeCdqvArKnFFBqejbg1UUl6mrvBSsgcGBAUE");
        private static int[] order = new int[] { 7,1,13,5,4,6,7,7,9,10,11,11,13,13,14 };
        private static int key = 5;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
