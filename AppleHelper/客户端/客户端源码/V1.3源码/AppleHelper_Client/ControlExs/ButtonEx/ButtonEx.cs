﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace AppleHelper_Client
{

    /// <summary>
    /// 仿QQ效果的Button
    /// </summary>
    public class ButtonEx : Button
    {
        #region Field

        private Image _normalImg = GetImageFormResourceStream("ControlExs.ButtonEx.Image.qqbtn_normal.png");
        private Image _highlightImg = GetImageFormResourceStream("ControlExs.ButtonEx.Image.qqbtn_highlight.png");
        private Image _focusImg = GetImageFormResourceStream("ControlExs.ButtonEx.Image.qqbtn_focus.png");
        private Image _downImg = GetImageFormResourceStream("ControlExs.ButtonEx.Image.qqbtn_down.png");

        private ControlExState _state = ControlExState.Normal;
        private Font _defaultFont = new Font("微软雅黑", 9);

        #endregion

        #region Constructor

        public ButtonEx()
        {
            SetStyles();
            this.Font = _defaultFont;
            this.Size = new Size(68, 23);
        }

        #endregion

        #region Properites

        private int ImageWidth
        {
            get
            {
                if (Image == null)
                {
                    return 16;
                }
                else
                {
                    return Image.Width;
                }
            }

        }

        #endregion

        #region Override

        protected override void OnMouseEnter(EventArgs e)
        {
            _state = ControlExState.Highlight;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_state == ControlExState.Highlight && Focused)
            {
                _state = ControlExState.Focus;
            }
            else if (_state == ControlExState.Focus)
            {
                _state = ControlExState.Focus;
            }
            else
            {
                _state = ControlExState.Normal;
            }
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                _state = ControlExState.Down;
            }
            this.Invalidate();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                if (ClientRectangle.Contains(mevent.Location))
                {
                    _state = ControlExState.Highlight;
                }
                else
                {
                    _state = ControlExState.Focus;
                }
            }
            this.Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _state = ControlExState.Normal;
            this.Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
            {
                _state = ControlExState.Normal;
            }
            else
            {
                _state = ControlExState.Disabled;
            }
            this.Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            Rectangle imageRect, textRect;
            CalculateRect(out imageRect, out textRect);

            if (!Enabled)
            {
                _state = ControlExState.Disabled;
            }
            switch (_state)
            {
                case ControlExState.Normal:

                    DrawImageWithNineRect(
                        g, _normalImg,
                        ClientRectangle,
                        new Rectangle(0, 0, _normalImg.Width, _normalImg.Height));
                    break;
                case ControlExState.Highlight:

                    DrawImageWithNineRect(
                        g, _highlightImg,
                        ClientRectangle,
                        new Rectangle(0, 0, _highlightImg.Width, _highlightImg.Height));
                    break;
                case ControlExState.Focus:

                    DrawImageWithNineRect(
                        g, _focusImg,
                        ClientRectangle,
                        new Rectangle(0, 0, _focusImg.Width, _focusImg.Height));
                    break;
                case ControlExState.Down:
                    DrawImageWithNineRect(
                       g, _downImg,
                       ClientRectangle,
                       new Rectangle(0, 0, _downImg.Width, _downImg.Height));
                    break;
                case ControlExState.Disabled:
                    DrawDisabledButton(g);
                    break;
                default:
                    break;
            }

            if (Image != null)
            {
                g.DrawImage(Image, imageRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            }

            Color textColor = Enabled ? ForeColor : SystemColors.GrayText;
            TextRenderer.DrawText(
                  g,
                  Text,
                  Font,
                  textRect,
                  textColor,
                  GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_normalImg != null) { _normalImg.Dispose(); }
                if (_highlightImg != null) { _highlightImg.Dispose(); }
                if (_downImg != null) { _downImg.Dispose(); }
                if (_focusImg != null) { _focusImg.Dispose(); }
                if (_defaultFont != null) { _defaultFont.Dispose(); }
            }

            _normalImg = null;
            _highlightImg = null;
            _focusImg = null;
            _downImg = null;
            _defaultFont = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private

        private void SetStyles()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        private static Image GetImageFormResourceStream(string imagePath)
        {
            return Image.FromStream(
                Assembly.GetExecutingAssembly().
                GetManifestResourceStream(
                MethodBase.GetCurrentMethod().DeclaringType.Namespace + "." + imagePath));
        }

        private void DrawImageWithNineRect(Graphics g, Image img, Rectangle targetRect, Rectangle srcRect)
        {
            int offset = 5;
            Rectangle NineRect = new Rectangle(img.Width / 2 - offset, img.Height / 2 - offset, 2 * offset, 2 * offset);
            int x = 0, y = 0, nWidth, nHeight;
            int xSrc = 0, ySrc = 0, nSrcWidth, nSrcHeight;
            int nDestWidth, nDestHeight;
            nDestWidth = targetRect.Width;
            nDestHeight = targetRect.Height;
            // 左上-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Top;
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = NineRect.Top - srcRect.Top;
            xSrc = srcRect.Left;
            ySrc = srcRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 上-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            nSrcHeight = NineRect.Top - srcRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 右上-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 左-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Top + NineRect.Top - srcRect.Top;
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = targetRect.Bottom - y - (srcRect.Bottom - NineRect.Bottom);
            xSrc = srcRect.Left;
            ySrc = NineRect.Top;
            nSrcWidth = NineRect.Left - srcRect.Left;
            nSrcHeight = NineRect.Bottom - NineRect.Top;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 中-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);

            // 右-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            nSrcWidth = srcRect.Right - NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);

            // 左下-------------------------------------;
            x = targetRect.Left;
            y = targetRect.Bottom - (srcRect.Bottom - NineRect.Bottom);
            nWidth = NineRect.Left - srcRect.Left;
            nHeight = srcRect.Bottom - NineRect.Bottom;
            xSrc = srcRect.Left;
            ySrc = NineRect.Bottom;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
            // 下-------------------------------------;
            x = targetRect.Left + NineRect.Left - srcRect.Left;
            nWidth = nDestWidth - nWidth - (srcRect.Right - NineRect.Right);
            xSrc = NineRect.Left;
            nSrcWidth = NineRect.Right - NineRect.Left;
            nSrcHeight = srcRect.Bottom - NineRect.Bottom;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nSrcWidth, nSrcHeight, GraphicsUnit.Pixel);
            // 右下-------------------------------------;
            x = targetRect.Right - (srcRect.Right - NineRect.Right);
            nWidth = srcRect.Right - NineRect.Right;
            xSrc = NineRect.Right;
            g.DrawImage(img, new Rectangle(x, y, nWidth, nHeight), xSrc, ySrc, nWidth, nHeight, GraphicsUnit.Pixel);
        }

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            if (Image == null)
            {
                textRect = new Rectangle(
                   3,
                   0,
                   Width - 6,
                   Height);
                return;
            }
            switch (TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    imageRect = new Rectangle(
                        3,
                        (Height - ImageWidth) / 2,
                        ImageWidth,
                        ImageWidth);
                    textRect = new Rectangle(
                        3,
                        0,
                        Width - 6,
                        Height);
                    break;
                case TextImageRelation.ImageAboveText:
                    imageRect = new Rectangle(
                        (Width - ImageWidth) / 2,
                        3,
                        ImageWidth,
                        ImageWidth);
                    textRect = new Rectangle(
                        3,
                        imageRect.Bottom,
                        Width - 6,
                        Height - imageRect.Bottom - 2);
                    break;
                case TextImageRelation.ImageBeforeText:
                    imageRect = new Rectangle(
                        3,
                        (Height - ImageWidth) / 2,
                        ImageWidth,
                        ImageWidth);
                    textRect = new Rectangle(
                        imageRect.Right + 3,
                        0,
                        Width - imageRect.Right - 6,
                        Height);
                    break;
                case TextImageRelation.TextAboveImage:
                    imageRect = new Rectangle(
                        (Width - ImageWidth) / 2,
                        Height - ImageWidth - 3,
                        ImageWidth,
                        ImageWidth);
                    textRect = new Rectangle(
                        0,
                        3,
                        Width,
                        Height - imageRect.Y - 3);
                    break;
                case TextImageRelation.TextBeforeImage:
                    imageRect = new Rectangle(
                        Width - ImageWidth - 6,
                        (Height - ImageWidth) / 2,
                        ImageWidth,
                        ImageWidth);
                    textRect = new Rectangle(
                        3,
                        0,
                        imageRect.X - 3,
                        Height);
                    break;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                imageRect.X = Width - imageRect.Right;
                textRect.X = Width - textRect.Right;
            }
        }

        private void DrawDisabledButton(Graphics g)
        {
            int radius = 4;
            //此处让其宽度减1，让其由Normal态平滑自然的过渡到Disabled态，保持按钮高度一致。
            using (GraphicsPath borderPath = CreateRoundPath(new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height - 1), radius))
            {
                using (Pen disalbedPen = new Pen(Color.FromArgb(156, 165, 177)))
                {
                    g.DrawPath(disalbedPen, borderPath);
                }

                //背景层渐变,向内缩小1个像素
                Rectangle backRect = new Rectangle(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2 - 1);
                using (GraphicsPath innerPath = CreateRoundPath(backRect, radius))
                {
                    using (LinearGradientBrush lBrush = new LinearGradientBrush(backRect, Color.FromArgb(247, 252, 254), Color.FromArgb(230, 240, 243), LinearGradientMode.Vertical))
                    {
                        g.FillPath(lBrush, innerPath);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int radiusCorrection = 1;
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(
                rect.Right - radius - radiusCorrection,
                rect.Y,
                radius,
                radius,
                270,
                90);
            path.AddArc(
                rect.Right - radius - radiusCorrection,
                rect.Bottom - radius - radiusCorrection,
                radius,
                radius, 0, 90);
            path.AddArc(
                rect.X,
                rect.Bottom - radius - radiusCorrection,
                radius,
                radius,
                90,
                90);
            path.CloseFigure();
            return path;
        }

        internal static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment, bool rightToleft)
        {
            TextFormatFlags flags = TextFormatFlags.WordBreak |
                TextFormatFlags.SingleLine;
            if (rightToleft)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
            }
            return flags;
        }

        #endregion
    }
}
