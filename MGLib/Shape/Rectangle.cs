using MonoGame.Extended;

namespace MGLib
{
    public class Rectangle
    {
        //public RectangleF Rect;
        public Microsoft.Xna.Framework.Rectangle Rect; // Can's have this private, or readonly...(?) Damn structs!
        private short _categoryBits, _maskBits;
        
        //public Rectangle(float x, float y, float width, float height, short categoryBits, short maskBits)
        public Rectangle(int x, int y, int width, int height, short categoryBits, short maskBits)
        {
            //Rect = new RectangleF(x, y, width, height);
            Rect = new Microsoft.Xna.Framework.Rectangle(x, y, width, height);
            _categoryBits = categoryBits;
            _maskBits = maskBits;
        }

        public short CategoryBits => _categoryBits;

        public short MaskBits => _maskBits;
    }
}
