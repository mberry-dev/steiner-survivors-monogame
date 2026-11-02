using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameSoundtrack
{
    private List<Song> songs;  // List to store all songs
    private ContentManager content;
    private int currentSongIndex = 0;  // Keeps track of the current song
    private Random rng = new Random();  // Random number generator for selecting songs
    private bool isMusicPaused = false; // To track if music is paused
    private Song currentPlayingSong = null; // To store the current song playing
    
    // Define songs as private variables
    private Song sendItSong;
    private Song burningSong;
    private Song byobSong;
    private Song calabriaSong;
    private Song came_forSong;
    private Song cant_stopSong;
    private Song celeritySong;
    private Song dreamersSong;
    private Song drownSong;
    private Song endlessSong;
    private Song fridaySong;
    private Song friendshipsSong;
    private Song in_the_endSong;
    private Song jump_aroundSong;
    private Song let_it_rockSong;
    private Song lets_goSong;
    private Song level_upSong;
    private Song look_at_meSong;
    private Song no_timeSong;
    private Song one_more_timeSong;
    private Song play_hardSong;
    private Song rule_the_worldSong;
    private Song somewhere_i_belongSong;
    private Song survivalSong;
    private Song triangle_violinSong;
    private Song truthSong;

    public GameSoundtrack(ContentManager content)
    {
        this.content = content;
        songs = new List<Song>();  // Initialize the song list

        LoadSongs();  // Load all songs
    }

    // Loads a song if it was built into the game, or returns null if it wasn't.
    // The music files are not included in the public repository (copyright),
    // so the game must keep running without them.
    private Song TryLoad(string name)
    {
        try
        {
            return content.Load<Song>("Audio/" + name);
        }
        catch (ContentLoadException)
        {
            return null;
        }
    }

    private void LoadSongs()
    {
        // Load each song
        sendItSong = TryLoad("send_it");
        burningSong = TryLoad("burning");
       // byobSong = TryLoad("byob");
        calabriaSong = TryLoad("calabria");
      //  came_forSong = TryLoad("came_for");
      //  cant_stopSong = TryLoad("cant_stop");
       // celeritySong = TryLoad("celerity");
        dreamersSong = TryLoad("dreamers");
       // drownSong = TryLoad("drown");
       // endlessSong = TryLoad("endless");
     //   fridaySong = TryLoad("friday");
       // friendshipsSong = TryLoad("friendships");
      //  in_the_endSong = TryLoad("in_the_end");
      //  jump_aroundSong = TryLoad("jump_around");
      //  let_it_rockSong = TryLoad("let_it_rock");
      //  lets_goSong = TryLoad("lets_go");
      //  level_upSong = TryLoad("level_up");
      //  look_at_meSong = TryLoad("look_at_me");
        no_timeSong = TryLoad("no_time");
       // one_more_timeSong = TryLoad("one_more_time");
       // play_hardSong = TryLoad("play_hard");
      //  rule_the_worldSong = TryLoad("rule_the_world");
      //  somewhere_i_belongSong = TryLoad("somewhere_i_belong");
       // survivalSong = TryLoad("survival");
        triangle_violinSong = TryLoad("triangle_violin");
      //  truthSong = TryLoad("truth");

        // Add all songs to the list
        songs.Add(sendItSong);
        songs.Add(burningSong);
        songs.Add(byobSong);
        songs.Add(calabriaSong);
        songs.Add(came_forSong);
        songs.Add(cant_stopSong);
        songs.Add(celeritySong);
        songs.Add(dreamersSong);
        songs.Add(drownSong);
        songs.Add(endlessSong);
        songs.Add(fridaySong);
        songs.Add(friendshipsSong);
        songs.Add(in_the_endSong);
        songs.Add(jump_aroundSong);
        songs.Add(let_it_rockSong);
        songs.Add(lets_goSong);
        songs.Add(level_upSong);
        songs.Add(look_at_meSong);
        songs.Add(no_timeSong);
        songs.Add(one_more_timeSong);
        songs.Add(play_hardSong);
        songs.Add(rule_the_worldSong);
        songs.Add(somewhere_i_belongSong);
        songs.Add(survivalSong);
        songs.Add(triangle_violinSong);
        songs.Add(truthSong);
    }



    public void PlayRandom()
    {
        if (songs == null || songs.Count == 0)
            return;

        // Filter out null songs
        var validSongs = songs.Where(song => song != null).ToList();
        if (validSongs.Count == 0)
            return;

        var random = new Random();
        Song selectedSong = validSongs[random.Next(validSongs.Count)];

        MediaPlayer.Play(selectedSong);
    }

    // Method to play a random song
    /*public void PlayRandom()
    {
        if (songs.Count == 0) return;

        // Get a random song index
        int randomIndex = rng.Next(songs.Count);
        currentSongIndex = randomIndex;
        MediaPlayer.Play(songs[randomIndex]);  // Play the random song
        isMusicPaused = false; // Reset pause state when a new song is played
    }*/

    // Method to play a specific song by its index
    public void PlaySong(int index)
    {
        if (index >= 0 && index < songs.Count && songs[index] != null)
        {
            currentSongIndex = index;
            MediaPlayer.Play(songs[currentSongIndex]);
            isMusicPaused = false; // Reset pause state when a new song is played
        }
    }

    // Method to play the next song in the list
    public void PlayNext()
    {
        if (!songs.Any(song => song != null)) return;

        // Skip over songs that were not loaded
        do
        {
            currentSongIndex = (currentSongIndex + 1) % songs.Count;
        } while (songs[currentSongIndex] == null);

        MediaPlayer.Play(songs[currentSongIndex]);
    }

    // Method to play the "send it" song
    public void PlaySendIt()
    {
        if (sendItSong == null) return; // No music files available

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(sendItSong);
        isMusicPaused = false; // Reset pause state when a new song is played
    }

    // Method to stop the current song
    public void Stop()
    {
        MediaPlayer.Stop();
        isMusicPaused = false; // Reset pause state when a new song is played
    }

    // Method to pause the music
    public void Pause()
    {
        if (!isMusicPaused)
        {
            MediaPlayer.Pause();
            isMusicPaused = true; // Mark that music is paused
        }
    }

    // Method to resume the music
    public void Resume()
    {
        if (isMusicPaused)
        {
            MediaPlayer.Resume();
            isMusicPaused = false; // Reset pause state
        }
    }
    public bool IsMusicPaused() => isMusicPaused;
}
