using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText, feed;
    CarController car;
    float score;
    int displayScore;
    [SerializeField] float chainBreakTime, lastEarnTime, lastChainEarn, minimumScore, currentChainScore;
    [SerializeField] List<string> chainTitles;
    [SerializeField] int[] chainThresholds, chainRewards; // the thresholds we need to break
    int currentChainPosition;

    // get our car
    private void Start()
    {
        car = CarController.instance;
    }

    // script is used to track our score
    private void Update()
    {
        // process the score every tick
        ProcessScore();
        ProcessChains();
    }

    // processes our score every update tick
    void ProcessScore()
    {
        score += ScoreToAdd();
        currentChainScore += ScoreToAdd();
        displayScore = (int)score;
        scoreText.text = displayScore.ToString();
    }

    // processes our drift chains
    void ProcessChains()
    {
        // if our current chain score exceeds a certain threshold, add to the chain
        if (currentChainScore > chainThresholds[currentChainPosition])
        {
            AdvanceChain();
        }

        // if the score to add is less than the minimum score, and time.time is more than our last score earned + chainBreakTime, then break the chain
        if (ScoreToAdd() < minimumScore && Time.time > lastEarnTime + chainBreakTime)
        {
            BreakChain();
        }
    }

    // checks how much score we want to add
    float ScoreToAdd()
    {
        if (score > minimumScore)
            lastEarnTime = Time.time;

        return car.sidewaysSpeed * 10;
    }

    void BreakChain()
    {
        lastEarnTime = Time.time;
        lastChainEarn = 0;
        currentChainPosition = 0;
        currentChainScore = 0;
    }

    // advances the chain we're currently on
    void AdvanceChain()
    {
        currentChainPosition++;
        score += chainRewards[currentChainPosition];
        // then add that to our feed
        AddToFeed(chainTitles[currentChainPosition] + " - " + chainRewards[currentChainPosition]);
    }

    void AddToFeed(string text)
    {
        feed.text =  text + "\n" + feed.text;
    }
}
