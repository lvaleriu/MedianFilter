﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YLScsImage;

namespace img
{
    public partial class MainForm : Form
    {
        private readonly Convertor convertor;
        private readonly Settings settings;
        private readonly ToolTip t1 = new ToolTip();
        private readonly ToolTip t2 = new ToolTip();
        private readonly List<TabPage> tabs;

        public MainForm()
        {
            InitializeComponent();
            try
            {
                tabs = new List<TabPage>();
                tabControl1.Selected += tabControl1_Selected;
                convertor = new Convertor();
                settings = new Settings(3, 15, 0);
                timer1.Interval = 1000;
                timer1.Start();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenLogic(TabPage tab)
        {
            try
            {
                median.Enabled = true;
                mmedian.Enabled = true;
                noise.Enabled = true;
                mnoise.Enabled = true;
                if (tab.Tag.ToString() == ".bmp")
                {
                    convert.Enabled = true;
                    mconvert.Enabled = true;
                    mconvert2.Enabled = false;
                    convert2.Enabled = false;
                }
                else if (tab.Tag.ToString() == ".sci")
                {
                    convert2.Enabled = true;
                    convert.Enabled = false;
                    mconvert.Enabled = false;
                    mconvert2.Enabled = true;
                }

                TabPage tp = tabControl1.SelectedTab;
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                if (pb2.Image != null)
                {
                    save.Enabled = true;
                    msave.Enabled = true;
                }
                else
                {
                    save.Enabled = false;
                    msave.Enabled = false;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            OpenLogic(e.TabPage);
        }

        private void CreateTab(Image img, string ext = ".bmp")
        {
            try
            {
                var etalon = new TabPage();
                etalon.Text = string.Format("Img{0}", tabs.Count + 1);
                etalon.Tag = ext;

                var split = new SplitContainer();
                split.Dock = DockStyle.Fill;
                split.BorderStyle = BorderStyle.Fixed3D;
                split.SplitterDistance = split.Width/2;

                var pb1 = new ImagePanel();
                pb1.MouseWheel += pb1_MouseWheel;
                if (ext == ".bmp")
                {
                    t1.SetToolTip(pb1, "Формат изображения BMP");
                    pb1.Tag = ".bmp";
                }
                else if (ext == ".sci")
                {
                    t1.SetToolTip(pb1, "Формат изображения SCI");
                    pb1.Tag = ".sci";
                }
                pb1.Dock = DockStyle.Fill;
                pb1.Name = string.Format("pb{0}", tabs.Count);
                if (img != null)
                    pb1.Image = new Bitmap(img);
                split.Panel1.Controls.Add(pb1);

                var pb2 = new ImagePanel();
                pb2.MouseWheel += pb1_MouseWheel;
                pb2.Dock = DockStyle.Fill;
                pb2.Name = string.Format("pb{0}", tabs.Count + 1);
                split.Panel2.Controls.Add(pb2);

                etalon.Controls.Add(split);
                tabControl1.TabPages.Add(etalon);
                tabs.Add(etalon);
                zoomin.Enabled = true;
                zoomout.Enabled = true;
                OpenLogic(etalon);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pb1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                if (e.Delta > 0)
                {
                    if (pb1.Image != null)
                    {
                        pb1.Zoom += 0.05f;
                    }
                    if (pb2.Image != null)
                    {
                        pb2.Zoom += 0.05f;
                    }
                }
                else
                {
                    if (pb1.Image != null)
                    {
                        pb1.Zoom -= 0.05f;
                    }
                    if (pb2.Image != null)
                    {
                        pb2.Zoom -= 0.05f;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void open_Click(object sender, EventArgs e)
        {
            try
            {
                var open = new OpenFileDialog();
                open.Filter = "bitmap|*.bmp|SCI|*.sci";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = default(Bitmap);
                    if (open.FileName.IndexOf(".sci") != -1)
                    {
                        bmp = new Bitmap(convertor.OpenSCI(open.FileName));
                    }
                    else if (open.FileName.IndexOf(".bmp") != -1)
                    {
                        bmp = new Bitmap(open.FileName);
                    }

                    CreateTab(new Bitmap(bmp), open.FileName.Substring(open.FileName.LastIndexOf(".")));
                    tabControl1.SelectedIndex = tabs.Count - 1;
                    right.Enabled = false;
                    left.Enabled = false;
                    tool2.Text = string.Format("{0}х{1}", bmp.Width, bmp.Height);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.AppStarting;
                TabPage tp = tabControl1.SelectedTab;
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                var save = new SaveFileDialog();
                if (pb2.Tag.ToString() == ".sci")
                {
                    save.DefaultExt = ".sci";
                    save.Filter = "SCI|*.sci";
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        convertor.SetBMP = pb2.Image;
                        convertor.SaveOnDisk(save.FileName);
                    }
                }
                else if (pb2.Tag.ToString() == ".bmp")
                {
                    save.DefaultExt = ".bmp";
                    save.Filter = "Bitmap|*.bmp";
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        pb2.Image.Save(save.FileName);
                    }
                }
                Cursor = Cursors.Default;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void median_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dold = DateTime.Now;
                Cursor = Cursors.AppStarting;
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                IFilter median;
                if (settings.IsCudafyEngine)
                {
                    median = new CudafyMedianFilter(new Bitmap(pb1.Image), settings.VideoMemorySize<<10,settings.Median);
                }
                else if (settings.IsMpiEngine)
                {
                    median = new MpiMedianFilter(new Bitmap(pb1.Image), settings.NumberOfProcess,settings.Median);
                }
                else
                {
                    median = new MedianFilter(new Bitmap(pb1.Image), settings.Median);
                }
                median.Filter();
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                TimeSpan sp = DateTime.Now - dold;
                MessageBox.Show(string.Format("Время фильтрации: {0}\n", sp));

                #region params

                if (tp.Tag.ToString() == ".sci")
                {
                    t1.SetToolTip(pb1, "Формат изображения SCI");
                    t2.SetToolTip(pb2, "Формат изображения SCI");
                    pb1.Tag = ".sci";
                    pb2.Tag = ".sci";
                }

                if (tp.Tag.ToString() == ".bmp")
                {
                    t1.SetToolTip(pb1, "Формат изображения BMP");
                    t2.SetToolTip(pb2, "Формат изображения BMP");
                    pb1.Tag = ".bmp";
                    pb2.Tag = ".bmp";
                }

                pb2.Image = median.getNewBMP;
                OpenLogic(tp);

                left.Enabled = true;
                right.Enabled = true;
                pb1.Zoom = 1;
                pb2.Zoom = 1;

                #endregion

                Cursor = Cursors.Default;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Кодирование изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convert_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.AppStarting;
                TabPage tp = tabControl1.SelectedTab;
                if (tp.Tag.ToString() == ".bmp")
                {
                    var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                    var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                    convertor.SetBMP = pb1.Image;
                    convertor.SetAlfa = settings.SCI;
                    pb2.Image = new Bitmap(convertor.ConvertToSCI());

                    #region params

                    pb2.Tag = ".sci";
                    pb1.Tag = ".bmp";
                    t1.SetToolTip(pb1, "Формат изображения BMP");
                    t2.SetToolTip(pb2, "Формат изображения SCI");
                    left.Enabled = true;
                    right.Enabled = true;
                    save.Enabled = true;
                    msave.Enabled = true;
                    pb1.Zoom = 1;
                    pb2.Zoom = 1;

                    #endregion
                }
                Cursor = Cursors.Default;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void noise_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.AppStarting;
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var filter = new MedianFilter();
                pb1.Image = new Bitmap(filter.AddNoise(pb1.Image, settings.Noise));

                #region params

                if (tp.Tag.ToString() == ".bmp")
                {
                    t1.SetToolTip(pb1, "Формат изображения BMP");
                }
                else if (tp.Tag.ToString() == ".sci")
                {
                    t1.SetToolTip(pb1, "Формат изображения SCI");
                }

                #endregion

                Cursor = Cursors.Default;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void left_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                pb1.Image = new Bitmap(pb2.Image);
                pb2.Image = null;

                #region params

                tp.Tag = pb2.Tag;
                if (pb2.Tag.ToString() == ".bmp")
                {
                    t1.SetToolTip(pb1, "Формат изображения BMP");
                    pb1.Tag = ".bmp";
                    convert.Enabled = true;
                    convert2.Enabled = false;
                    mconvert.Enabled = true;
                    mconvert2.Enabled = false;
                }
                if (pb2.Tag.ToString() == ".sci")
                {
                    t1.SetToolTip(pb1, "Формат изображения SCI");
                    pb1.Tag = ".sci";
                    convert.Enabled = false;
                    convert2.Enabled = true;
                    mconvert.Enabled = false;
                    mconvert2.Enabled = true;
                }
                if (pb2.Image == null)
                {
                    save.Enabled = false;
                    msave.Enabled = false;
                }
                else
                {
                    save.Enabled = true;
                    msave.Enabled = true;
                }
                median.Enabled = true;
                mmedian.Enabled = true;
                noise.Enabled = true;
                mnoise.Enabled = true;
                left.Enabled = false;
                right.Enabled = true;

                #endregion
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void right_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                pb2.Image = new Bitmap(pb1.Image);
                pb1.Image = null;

                #region params

                tp.Tag = "";
                if (pb1.Tag.ToString() == ".bmp")
                {
                    t2.SetToolTip(pb2, "Формат изображения BMP");
                    pb2.Tag = ".bmp";
                }
                if (pb1.Tag.ToString() == ".sci")
                {
                    t2.SetToolTip(pb2, "Формат изображения SCI");
                    pb2.Tag = ".sci";
                }
                if (pb2.Image == null)
                {
                    save.Enabled = false;
                    msave.Enabled = false;
                }
                else
                {
                    save.Enabled = true;
                    msave.Enabled = true;
                }
                median.Enabled = false;
                mmedian.Enabled = false;
                convert.Enabled = false;
                mconvert.Enabled = false;
                mconvert2.Enabled = false;
                noise.Enabled = false;
                mnoise.Enabled = false;
                left.Enabled = true;
                right.Enabled = false;

                #endregion
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (settings.ShowDialog() == DialogResult.OK)
            {
                // good
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                if (pb1.Image != null)
                {
                    pb1.Zoom += 0.05f;
                }
                if (pb2.Image != null)
                {
                    pb2.Zoom += 0.05f;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage tp = tabControl1.SelectedTab;
                var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                if (pb1.Image != null)
                {
                    pb1.Zoom -= 0.05f;
                }
                if (pb2.Image != null)
                {
                    pb2.Zoom -= 0.05f;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void запускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exp = new Exp();
            if (exp.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open_Click(sender, e);
        }

        private void mmedian_Click(object sender, EventArgs e)
        {
            median_Click(sender, e);
        }

        private void mconvert_Click(object sender, EventArgs e)
        {
            convert_Click(sender, e);
        }

        private void mnoise_Click(object sender, EventArgs e)
        {
            noise_Click(sender, e);
        }

        private void msave_Click(object sender, EventArgs e)
        {
            save_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tool1.Text = DateTime.Now.ToString();
        }


        /// <summary>
        ///     Кодирование изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.AppStarting;
                TabPage tp = tabControl1.SelectedTab;
                if (tp.Tag.ToString() == ".sci")
                {
                    var pb1 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel1.Controls[0];
                    var pb2 = (ImagePanel) ((SplitContainer) tp.Controls[0]).Panel2.Controls[0];
                    convertor.SetBMP = pb1.Image;
                    convertor.ConvertToSCI();
                    pb2.Image = convertor.decode(convertor.getSCI); // Кодирование изображения в BMP

                    #region params

                    pb2.Tag = ".bmp";
                    pb1.Tag = ".sci";
                    t1.SetToolTip(pb1, "Формат изображения SCI");
                    t2.SetToolTip(pb2, "Формат изображения BMP");
                    left.Enabled = true;
                    right.Enabled = true;
                    save.Enabled = true;
                    msave.Enabled = true;
                    pb1.Zoom = 1;
                    pb2.Zoom = 1;

                    #endregion
                }
                Cursor = Cursors.Default;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Окно О программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ab = new About();
            ab.ShowDialog();
        }
    }
}