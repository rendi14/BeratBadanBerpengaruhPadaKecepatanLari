using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using PusingYaAllah;
using UnityEditor;

public class RegresiMain : MonoBehaviour
{
    public TextAsset jsonText = Constanta.jsonFile;
    public Text outputTextObject;
    public Window_Graph winGraph;

    private int input_x = int.Parse(PusingYaAllah.Constanta.nilaiX.ToString());
    

    private int banyakN;
    private int sumX, sumX2;
    private int sumY, sumY2;
    private int sumXY;
    
    private double Nilai_Konstanta;
    private double Nilai_Koefisien;
    private double Nilai_Korelasi;

    private static double gradient;
    public static int max_y, max_x;

    private List<int> data_x = new List<int>();
    private List<int> data_y = new List<int>();

    [Serializable]
    public class DataSet {
        public Values[] Values;
    }
    
    [Serializable]
    public class Values
    {
        public int BeratBadan;
        public int KecepatanLari;
    }

    void Start()
    {
        Regresi();
        ShowData();
        Export();
    }

    public void Regresi()
    {
        DataSet data = JsonUtility.FromJson<DataSet>(jsonText.text);
        int sum_x = 0;
        int sum_y = 0;
        int sum_x2 = 0;
        int sum_y2 = 0;
        int sum_xy = 0;
        int n = data.Values.Length;

        for (int i = 0; i < n; i++)
        {
            // grafik
            data_x.Add(Mathf.RoundToInt(data.Values[i].BeratBadan));
            data_y.Add(Mathf.RoundToInt(data.Values[i].KecepatanLari));
            
            if (i == 0)
            {
                max_y = Mathf.RoundToInt(data.Values[i].KecepatanLari);
                max_x = data.Values[i].BeratBadan;
            }
            else
            {
                if (max_y <= Mathf.RoundToInt(data.Values[i].KecepatanLari))
                {
                    max_y = Mathf.RoundToInt(data.Values[i].KecepatanLari);
                }
                if (max_x <= data.Values[i].BeratBadan)
                {
                    max_x = data.Values[i].BeratBadan;
                }
            }
            
            // perhitungan
            sum_x = sum_x + data.Values[i].BeratBadan;
            sum_y = sum_y + Mathf.RoundToInt(data.Values[i].KecepatanLari);
            sum_x2 += data.Values[i].BeratBadan * data.Values[i].BeratBadan;
            sum_y2 += Mathf.RoundToInt(data.Values[i].KecepatanLari) * Mathf.RoundToInt(data.Values[i].KecepatanLari);
            sum_xy += data.Values[i].BeratBadan * Mathf.RoundToInt(data.Values[i].KecepatanLari);
        }

        winGraph.GetValues(data_x, data_y);
        winGraph.GraphCall();

        this.banyakN = n;
        this.sumX = sum_x;
        this.sumY = sum_y;
        this.sumX2 = sum_x2;
        this.sumY2 = sum_y2;
        this.sumXY = sum_xy;

        double konstanta = this.Konstanta();
        this.Nilai_Konstanta = konstanta;

        double koefisien = this.Koefisien();
        this.Nilai_Koefisien = koefisien;

        double korelasi = this.Korelasi();
        this.Nilai_Korelasi = korelasi;

        string hubungan = this.Hubungan();

        string kekuatan = this.Kekuatan();

        double koefisien_determinasi = this.Koefisien_determinasi();

        double kontrib_var_lain = this.Kontribusi_var_lain();

    }

    public double Konstanta()
    {
        int atas = ((this.sumY * this.sumX2) - (this.sumX * this.sumXY));
        double bawah = (this.banyakN) * (this.sumX2) - Math.Pow(this.sumX, 2);
        double hasilKonstanta = atas / bawah;
        return hasilKonstanta;

    }

    public double Koefisien()
    {
        int atas = (this.banyakN * this.sumXY) - (this.sumX * this.sumY);
        double bawah = (this.banyakN * this.sumX2) - Math.Pow(this.sumX, 2);
        double hasilKoefisien = atas / bawah;
        return hasilKoefisien;
    }

    public double Korelasi()
    {
        int atas = (this.banyakN * this.sumXY) - (this.sumX * this.sumY);
        double bawah_kiri = (this.banyakN * this.sumX2) - Math.Pow(this.sumX, 2);
        double bawah_kanan = (this.banyakN * this.sumY2) - Math.Pow(this.sumY, 2);
        double hasil_bawah = Math.Sqrt(bawah_kiri * bawah_kanan);
        double hasilKorelasi = atas / hasil_bawah;
        return hasilKorelasi;
    }

    public string Hubungan()
    {
        var korelasi = Korelasi();
        if (korelasi < 0)
        {
            return "negatif";
        }
        else
        {
            return "positif";
        }
    }

    public string Kekuatan()
    {
        var kekuatan = Korelasi();
        if (Math.Abs(kekuatan) < 0.2)
        {
            return "sangat lemah";
        }
        else if (Math.Abs(kekuatan) >= 0.2 && Math.Abs(kekuatan) < 0.4)
        {
            return "lemah";
        }
        else if (Math.Abs(kekuatan) >= 0.4 && Math.Abs(kekuatan) < 0.6)
        {
            return "sedang";
        }
        else if (Math.Abs(kekuatan) >= 0.6 && Math.Abs(kekuatan) < 0.8)
        {
            return "kuat";
        }
        else if (Math.Abs(kekuatan) >= 0.8 && Math.Abs(kekuatan) <= 1)
        {
            return "sangat kuat";
        }
        else
        {
            return "tidak terdefinisi";
        }
    }

    public double Koefisien_determinasi()
    {
        double koef_deter = Math.Round(Math.Pow(this.Korelasi(), 2) * 100, 1);
        return koef_deter;
    }

    public double Kontribusi_var_lain()
    {
        double kontribusi = Math.Round(100 - this.Koefisien_determinasi(), 1);
        return kontribusi;
    }

    void ShowData()
    {
        string outputText = "";
        string hubungan = Hubungan();
        string kekuatan = Kekuatan();
        string rKekuatan = "";
        outputText += "152018066 M.Scandy Pradapta P";
        outputText += "\n";
        outputText += "\nNilai Berat Badan Masukan: " + Constanta.nilaiX;
        outputText += "\n";
        outputText += "\nNilai Konstanta: " + Math.Round(Nilai_Konstanta,2);
        outputText += "\nNilai Koefisien: " + Math.Round(Nilai_Koefisien,2);
        outputText += "\nNilai Korelasi: " + Math.Round(Nilai_Korelasi, 5);
        outputText += "\nNilai Hubungan: " + hubungan;
        outputText += "\nNilai Kekuatan: " + kekuatan;
        outputText += "\nNilai Koefisien Determinasi: " + Koefisien_determinasi() + "%";
        outputText += "\nNilai Kontribusi Variable Lain: " + Kontribusi_var_lain();
        outputText += "\nNilai Gradient: " + Math.Round(gradient, 2);

        switch (kekuatan)
        {
            case "sangat lemah":
                rKekuatan = "tidak berpengaruh";
                break;

            case "lemah":
                rKekuatan = "kurang berpengaruh";
                break;

            case "sedang":
                rKekuatan = "sedikit berpengaruh";
                break;

            case "kuat":
                rKekuatan = "berpengaruh";
                break;

            case "sangat kuat":
                rKekuatan = "sangat berpengaruh";
                break;

            default:
                break;
        }

        outputText += "\nKeterangan: Maka dapat disimpulkan bahwa variable berat badan memiliki hubungan/kekuatan yang " + kekuatan + " dengan kecepatan lari, sehingga berat badan " + rKekuatan  + " dengan kecepatan lari";
        outputTextObject.text = outputText;
    }

    public void Export()
    {

        string hubungan = Hubungan();
        string kekuatan = Kekuatan();
        string rKekuatan = "";

        string filename = Application.dataPath.Replace("/Assets", "") + "/exportResult.csv";

        TextWriter tw = new StreamWriter(filename, false);
        tw.WriteLine("152018066 M.Scandy Pradapta P");
        tw.WriteLine("");
        tw.WriteLine("Nilai Berat Badan Masukan: " + Constanta.nilaiX);
        tw.WriteLine("");
        tw.WriteLine("SUM X: " + sumX);
        tw.WriteLine("SUM Y: " + sumY);
        tw.WriteLine("SUM X^2: " + sumX2);
        tw.WriteLine("SUM Y^2: " + sumY2);
        tw.WriteLine("SUM X*Y: " + sumXY);
        tw.WriteLine("Konstanta a: " + Nilai_Konstanta);
        tw.WriteLine("Koefisien b: " + Nilai_Koefisien);
        tw.WriteLine("Korelasi: " + Nilai_Korelasi);
        tw.WriteLine("Hubungan: " + hubungan);
        tw.WriteLine("Kekuatan: " + kekuatan);
        tw.WriteLine("Koefisien Determinasi: " + Koefisien_determinasi() + "%");
        tw.WriteLine("Kontribusi Variable Lain: " + Kontribusi_var_lain());
        tw.WriteLine("Input x: " + input_x);
        tw.WriteLine("Hasil gradient y = a + bx adalah: " + Math.Round(gradient, 2));

        switch (kekuatan)
        {
            case "sangat lemah":
                rKekuatan = "tidak berpengaruh";
                break;

            case "lemah":
                rKekuatan = "kurang berpengaruh";
                break;

            case "sedang":
                rKekuatan = "sedikit berpengaruh";
                break;

            case "kuat":
                rKekuatan = "berpengaruh";
                break;

            case "sangat kuat":
                rKekuatan = "sangat berpengaruh";
                break;

            default:
                break;
        }

        tw.WriteLine("Keterangan: Maka dapat disimpulkan bahwa variable berat badan memiliki hubungan/kekuatan yang " + kekuatan + " dengan kecepatan lari, sehingga berat badan " + rKekuatan + " dengan kecepatan lari");
        tw.Close();

        tw = new StreamWriter(filename, true);

        tw.Close();
        Debug.Log("Data exported successfully");
    }
}

    

