using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    class Button
    {
        public M_Rectangle rect;
        public Action<Button> onClick;
        public Color backColor;

        public Button(M_Rectangle _rect, Action<Button> _onClick)
        {
            this.rect = _rect;
            this.onClick = _onClick;

            backColor = Color.Black;
        }

        public virtual void Draw(Vector2 _mousePos)
        {
            int state = 0;
            if (rect.ColVector(_mousePos))
            {
                state = 1;

                if (Input.mbLeft.down)
                {
                    state = 2;
                }
            }
            
            rect.Draw(state == 0 ? backColor : state == 1 ? Color.Gray : Color.White);
        }

        public virtual void Update(Vector2 _mousePos)
        {
            if (Input.mbLeft.pressed && rect.ColVector(_mousePos))
            {
                onClick(this);

                Sounds.clickButton.Play();
            }
        }
    }

    class Btn_Text : Button
    {
        public string text;
        public SpriteFont font;

        Anchor anchor;

        public Btn_Text(M_Rectangle _rect, Action<Btn_Text> _onClick, SpriteFont _font, string _text)
            : base(_rect,f => _onClick((Btn_Text)f))
        {
            font = _font;
            text = _text;

            anchor = _rect.GetCenterAnchor();
        }

        public override void Draw(Vector2 _mousePos)
        {
            base.Draw(_mousePos);

            font.Draw(text, anchor);
        }
    }

    class Btn_Texture : Button
    {
        public Texture2D tex;

        Vector2 texPos;

        public Btn_Texture(M_Rectangle _rect, Action<Btn_Texture> _onClick, Texture2D _tex)
            : base(_rect, f => _onClick((Btn_Texture)f))
        {
            tex = _tex;

            texPos = _rect.GetCenter() - tex.GetSize() / 2f;
        }

        public override void Draw(Vector2 _mousePos)
        {
            base.Draw(_mousePos);

            tex.Draw(texPos);
        }
    }
}
