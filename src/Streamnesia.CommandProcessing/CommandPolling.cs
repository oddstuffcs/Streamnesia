using System;
using System.Collections.Generic;
using System.Linq;
using Streamnesia.Payloads;

namespace Streamnesia.CommandProcessing
{
    public class CommandPolling
    {
        private readonly Random _rng = new Random();
        private Payload[] _options;
        private Dictionary<string, int> _votes;

        public CommandPolling()
        {
            _options = new Payload[4];
            _votes = new Dictionary<string, int>();
        }

        public void GeneratePoll(IEnumerable<Payload> payloads)
        {
            for(var i = 0; i < _options.Length; i++)
                _options[i] = null;

            Payload candidate = payloads.Random(_rng);
            for(var i = 0; i < _options.Length; i++)
            {
                while(_options.Any(o => o != null && o.Name == candidate.Name))
                {
                    candidate = payloads.Random(_rng);
                }

                _options[i] = candidate;
            }

            _votes.Clear();
        }

        public void Vote(string username, int vote)
        {
            if(vote < 0 || vote >= _options.Length)
                return;

            if(_votes.ContainsKey(username))
            {
                _votes[username] = vote;
            }
            else
            {
                _votes.Add(username, vote);
            }
        }

        public Payload GetPayloadWithMostVotes()
        {
            if(!_votes.Any())
            {
                return _options.Random(_rng);
            }

            var mostVotedIndex = GetMostVotedIndex();
            return _options[mostVotedIndex];
        }

        public IEnumerable<PollOption> GetPollOptions()
        {
            return _options.Select((o, i) => new PollOption
            {
                Name = o.Name,
                Index = i,
                Votes = _votes.Count(v => v.Value == i)
            });
        }

        private int GetMostVotedIndex()
        {
            var votes = new int[_options.Count()];
            
            foreach(var vote in _votes)
            {
                votes[vote.Value]++;
            }

            int mostVotes = 0;
            int index = 0;
            for(var i = 0; i < votes.Length; i++)
            {
                if(votes[i] > mostVotes)
                {
                    mostVotes = votes[i];
                    index = i;
                }
            }

            return index;
        }
    }

    public static class EnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> input, Random rng)
        {
            return input.ElementAt(rng.Next(input.Count()));
        }
    }
}
