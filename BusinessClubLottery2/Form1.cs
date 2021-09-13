﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessClubLottery2.Events;
using BusinessClubLottery2.Settings;

namespace BusinessClubLottery2 {

    public partial class Form1 : Form {

        private static Path path = new Path();

        public Form1() {

            InitializeComponent();

            if (!(File.Exists(path.FILE))) {
                FileManagement.CreateDirectory(path.FILE);
            }

            if (!(File.Exists(path.CACHE))) {
                FileManagement.CreateFile(path.CACHE);
            }

            if (!(File.Exists(path.HISTORY))) {
                FileManagement.CreateFile(path.HISTORY);
            }

            if (!(File.Exists(path.NAMELIST))) {
                FileManagement.CreateFile(path.NAMELIST);
                FileManagement.Overwritetxt(path.NAMELIST, Resources.namelist_default);

            }

            if (!(File.Exists(path.PROPERTIES))) {
                FileManagement.CreateFile(path.PROPERTIES);
                FileManagement.Overwritetxt(path.PROPERTIES, Resources.properties_default);
            }
        }

        private void lotbtn_Click(object sender, EventArgs e) {

            int value = 0;

            try {
                value = int.Parse(valuebox.Text);
            } catch (Exception ex) {
                MessageBox.Show($"入力された値が不正です。\n\nException: {ex}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (value <= 0 || ListEvent.ToList(FileManagement.Readtxt(path.NAMELIST)).Length - 2 < value) {
                MessageBox.Show($"入力された値が不正です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(CheckSyntax())) {
                MessageBox.Show("名簿の書式が不正です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReferHistory.EditCache(value);

            string[] result = new string[] { null };

            resultbox.Text = ListEvent.ListToStr(result);
            ReferHistory.WriteHistory(result);
        }

        private bool CheckSyntax () {

            //bool containsStart = FileManagement.Readtxt(path.NAMELIST).Contains("start{");
            //bool containsEnd = FileManagement.Readtxt(path.NAMELIST).Contains("}end");

            string[] namelist = ListEvent.ToList2(FileManagement.Readtxt(path.NAMELIST));

            if (namelist[0].Contains("start{") && namelist[namelist.Length - 1].Contains("}end")) {
                return true;
            }

            return false;
        }

        private void settingbtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            Settings.Settings form = new Settings.Settings();
            form.ShowDialog(this);
            form.Dispose();
        }

        /// <summary>
        /// 
        /// 以下Lot実行関数
        /// Value - 結果個数指定あり
        /// History - cache書き込みあり
        /// 
        /// </summary>
        private static string[] Lot_withValue_withHistory (int value) {

            string[] result = new string[value];
            string[] rawresultlist = new string[value];
            string rawresult = "";

            while (true) {

                rawresult = ListEvent.ListToStr(ListEvent.RandomizeList(ListEvent.ToList(FileManagement.Readtxt(path.NAMELIST))));

                rawresultlist = rawresult.Split('\n');

                for (int i = 0; i <= value - 1; i++) {
                    result[i] = rawresultlist[i];
                }

                if (!(ReferHistory.IsOverlap(result, ListEvent.ToList2(FileManagement.Readtxt(path.CACHE))))) {

                    //for (int j = 0; j <= result.Length - 1; j++) {
                    //    MessageBox.Show(result[j].Replace("\r\n", "\n").Split(new[] { '\n', '\r' })[0] + "あ");
                    //}
                    break;
                }
                //MessageBox.Show("草");

            }

            ReferHistory.WriteCache(result);
            return result;
        }
    }
}