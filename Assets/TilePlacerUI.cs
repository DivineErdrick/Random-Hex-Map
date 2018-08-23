using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TilePlacerUI : MonoBehaviour {

    public int maxColumns = 10;
    Slider sliderColumns;
    InputField inputColumns;
    Text textColumnsPlace;
    Text textColumnsValue;
    int columns;
    public int Columns {
        get { return columns; }
    }

    public int maxRows = 10;
    Slider sliderRows;
    InputField inputRows;
    Text textRowsPlace;
    Text textRowsValue;
    int rows;
    public int Rows {
        get { return rows; }
    }

    Slider sliderPlainsWeight;
    InputField inputPlainsWeight;
    Text textPlainsWeightPlace;
    Text textPlainsWeightValue;
    int plainsWeight;
    public int PlainsWeight {
        get { return plainsWeight; }
    }

    Slider sliderPlainsRate;
    InputField inputPlainsRate;
    Text textPlainsRatePlace;
    Text textPlainsRateValue;
    int plainsRate;
    public int PlainsRate {
        get { return plainsRate; }
    }

    Slider sliderForestWeight;
    InputField inputForestWeight;
    Text textForestWeightPlace;
    Text textForestWeightValue;
    int forestWeight;
    public int ForesWeight {
        get { return forestWeight; }
    }

    Slider sliderForestRate;
    InputField inputForestRate;
    Text textForestRatePlace;
    Text textForestRateValue;
    int forestRate;
    public int ForestRate {
        get { return forestRate; }
    }

    // Use this for initialization
    void Start() {

        sliderColumns = GameObject.Find("Slider Columns").GetComponent<Slider>();
        Assert.IsNotNull(sliderColumns, "TilePlacerUI could not find the Slider Columns object.");
        inputColumns = GameObject.Find("Input Columns").GetComponent<InputField>();
        Assert.IsNotNull(inputColumns, "TilePlacerUI could not find the Input Columns object.");
        textColumnsPlace = GameObject.Find("Text Columns Place").GetComponent<Text>();
        Assert.IsNotNull(textColumnsPlace, "TilePlacerUI could not find the Text Columns Place object.");
        textColumnsValue = GameObject.Find("Text Columns Value").GetComponent<Text>();
        Assert.IsNotNull(textColumnsValue, "TilePlacerUI could not find the Text Columns Value object.");

        sliderRows = GameObject.Find("Slider Rows").GetComponent<Slider>();
        Assert.IsNotNull(sliderRows, "TilePlacerUI could not find the Slider Rows object.");
        inputRows = GameObject.Find("Input Rows").GetComponent<InputField>();
        Assert.IsNotNull(inputRows, "TilePlacerUI could not find the Input Rows object.");
        textRowsPlace = GameObject.Find("Text Rows Place").GetComponent<Text>();
        Assert.IsNotNull(textRowsPlace, "TilePlacerUI could not find the Text Rows Place object.");
        textRowsValue = GameObject.Find("Text Rows Value").GetComponent<Text>();
        Assert.IsNotNull(textRowsValue, "TilePlacerUI could not find the Text Rows Value object.");

        sliderPlainsWeight = GameObject.Find("Slider Plains Weight").GetComponent<Slider>();
        Assert.IsNotNull(sliderPlainsWeight, "TilePlacerUI could not find the Slider Plains Weight object.");
        inputPlainsWeight = GameObject.Find("Input Plains Weight").GetComponent<InputField>();
        Assert.IsNotNull(inputPlainsWeight, "TilePlacerUI could not find the Input Plains Weight object.");
        textPlainsWeightPlace = GameObject.Find("Text Plains Weight Place").GetComponent<Text>();
        Assert.IsNotNull(textPlainsWeightPlace, "TilePlacerUI could not find the Text Plains Weight Place object.");
        textPlainsWeightValue = GameObject.Find("Text Plains Weight Value").GetComponent<Text>();
        Assert.IsNotNull(textPlainsWeightValue, "TilePlacerUI could not find the Text Plains Weight Value object.");

        sliderPlainsRate = GameObject.Find("Slider Plains Rate").GetComponent<Slider>();
        Assert.IsNotNull(sliderPlainsRate, "TilePlacerUI could not find the Slider Plains Rate object.");
        inputPlainsRate = GameObject.Find("Input Plains Rate").GetComponent<InputField>();
        Assert.IsNotNull(inputPlainsRate, "TilePlacerUI could not find the Input Plains Rate object.");
        textPlainsRatePlace = GameObject.Find("Text Plains Rate Place").GetComponent<Text>();
        Assert.IsNotNull(textPlainsRatePlace, "TilePlacerUI could not find the Text Plains Rate Place object.");
        textPlainsRateValue = GameObject.Find("Text Plains Rate Value").GetComponent<Text>();
        Assert.IsNotNull(textPlainsRateValue, "TilePlacerUI could not find the Text Plains Rate Value object.");

        sliderForestWeight = GameObject.Find("Slider Forest Weight").GetComponent<Slider>();
        Assert.IsNotNull(sliderForestWeight, "TilePlacerUI could not find the Slider Forest Weight object.");
        inputForestWeight = GameObject.Find("Input Forest Weight").GetComponent<InputField>();
        Assert.IsNotNull(inputForestWeight, "TilePlacerUI could not find the Input Forest Weight object.");
        textForestWeightPlace = GameObject.Find("Text Forest Weight Place").GetComponent<Text>();
        Assert.IsNotNull(textForestWeightPlace, "TilePlacerUI could not find the Text Forest Weight Place object.");
        textForestWeightValue = GameObject.Find("Text Forest Weight Value").GetComponent<Text>();
        Assert.IsNotNull(textForestWeightValue, "TilePlacerUI could not find the Text Forsest Weight Value object.");

        sliderForestRate = GameObject.Find("Slider Forest Rate").GetComponent<Slider>();
        Assert.IsNotNull(sliderForestRate, "TilePlacerUI could not find the Slider Forest Rate object.");
        inputForestRate = GameObject.Find("Input Forest Rate").GetComponent<InputField>();
        Assert.IsNotNull(inputForestRate, "TilePlacerUI could not find the Input Forest Rate object.");
        textForestRatePlace = GameObject.Find("Text Forest Rate Place").GetComponent<Text>();
        Assert.IsNotNull(textForestRatePlace, "TilePlacerUI could not find the Text Forest Rate Place object.");
        textForestRateValue = GameObject.Find("Text Forest Rate Value").GetComponent<Text>();
        Assert.IsNotNull(textForestRateValue, "TilePlacerUI could not find the Text Forest Rate Value object.");

        columns = maxColumns;
        sliderColumns.maxValue = maxColumns;
        sliderColumns.value = maxColumns;
        textColumnsPlace.text = maxColumns.ToString();

        rows = maxRows;
        sliderRows.maxValue = maxRows;
        sliderRows.value = maxRows;
        textRowsPlace.text = maxRows.ToString();

        plainsWeight = Mathf.RoundToInt(sliderPlainsWeight.value);
        textPlainsWeightPlace.text = plainsWeight.ToString();

        plainsRate = Mathf.RoundToInt(sliderPlainsRate.value);
        textPlainsRatePlace.text = plainsRate.ToString();

        forestWeight = Mathf.RoundToInt(sliderForestWeight.value);
        textForestWeightPlace.text = forestWeight.ToString();

        forestRate = Mathf.RoundToInt(sliderForestRate.value);
        textForestRatePlace.text = forestRate.ToString();
    }

    public void SetValuesFromSliders () {

        columns = Mathf.RoundToInt(sliderColumns.value);
        inputColumns.text = columns.ToString();

        rows = Mathf.RoundToInt(sliderRows.value);
        inputRows.text = rows.ToString();

        plainsWeight = Mathf.RoundToInt(sliderPlainsWeight.value);
        inputPlainsWeight.text = plainsWeight.ToString();

        plainsRate = Mathf.RoundToInt(sliderPlainsRate.value);
        inputPlainsRate.text = plainsRate.ToString();

        forestWeight = Mathf.RoundToInt(sliderForestWeight.value);
        inputForestWeight.text = forestWeight.ToString();

        forestRate = Mathf.RoundToInt(sliderForestRate.value);
        inputForestRate.text = forestRate.ToString();
    }

    public void SetValuesFromInput (string sliderType) {

        switch (sliderType) {

            case "Columns":
                if (!int.TryParse(inputColumns.text, out columns)) {

                    columns = maxColumns;
                }
                if (columns > maxColumns) {

                    columns = maxColumns;
                }
                sliderColumns.value = columns;
                textColumnsValue.text = columns.ToString();
                break;

            case "Rows":
                if (!int.TryParse(inputRows.text, out rows)) {

                    rows = maxRows;
                }
                if (rows > maxRows) {

                    rows = maxRows;
                }
                sliderRows.value = rows;
                textRowsValue.text = rows.ToString();
                break;

            case "Plains Weight":
                if (! int.TryParse(inputPlainsWeight.text, out plainsWeight)) {

                    plainsWeight = 0;
                }
                sliderPlainsWeight.value = plainsWeight;
                textPlainsWeightValue.text = plainsWeight.ToString();
                break;

            case "Plains Rate":
                if (!int.TryParse(inputPlainsRate.text, out plainsRate)) {

                    plainsRate = 0;
                }
                sliderPlainsRate.value = plainsRate;
                textPlainsRateValue.text = plainsRate.ToString();
                break;

            case "Forest Weight":
                if (!int.TryParse(inputForestWeight.text, out forestWeight)) {

                    forestWeight = 0;
                }
                sliderForestWeight.value = forestWeight;
                textForestWeightValue.text = forestWeight.ToString();
                break;

            case "Forest Rate":
                if (!int.TryParse(inputForestRate.text, out forestRate)) {

                    forestRate = 0;
                }
                sliderForestRate.value = forestRate;
                textForestRateValue.text = forestRate.ToString();
                break;
            default:
                break;
        }
    }
    public void Quit () {
        Application.Quit();
    }
}
