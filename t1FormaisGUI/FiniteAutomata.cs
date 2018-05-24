using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace t1FormaisGUI
{
    public class FiniteAutomata : RegularLanguage
    {
        private Guid guid;
        private State initial;
        private SortedSet<State> states;
        private SortedSet<State> finals;
        private SortedSet<char> alphabet;
        //private Dictionary<T>;

        public FiniteAutomata(Builder builder) : base(inputType.FA)
        {
            initial = builder.initial;
            guid = builder.guid;
            states = new SortedSet<State>(builder.states.Values);
            finals = new SortedSet<State>(builder.finals);
            alphabet = new SortedSet<char>(builder.alphabet);

            Dictionary<TransitionInput, HashSet<State>> temp;
            temp = builder.transitions;
        }

        public override string getDefinition()
        {
            throw new NotImplementedException();
        }

        public override RegularGrammar getRG()
        {
            throw new NotImplementedException();
        }

        public override RegularExpression getRE()
        {
            throw new NotImplementedException();
        }
    }

    public class Builder
    {
        public Guid guid;
        private bool built;
        public State initial;
        public Dictionary<State, State> states;
        public HashSet<State> finals;
        public HashSet<char> alphabet;
        public Dictionary<TransitionInput, HashSet<State>> transitions;

        public Builder()
        {
            guid = Guid.NewGuid();
            built = false;
            initial = null;
            states = new Dictionary<State, State>();
            finals = new HashSet<State>();
            alphabet = new HashSet<char>();
            transitions = new Dictionary<TransitionInput, HashSet<State>>();
        }

        public State newState()
        {
            State state = new State(guid, states.Count());
            states.Add(state, state);
            return state;
        }

        public Builder importState(State state)
        {
            State imported = new State(state, guid, states.Count());
            if (states[imported] == null)
            {
                states.Add(imported, imported);
            }
            return this;
        }

        public Builder setInitial(State s)
        {
            s = this.validateState(s);
            initial = s;
            return this;
        }

        public Builder setFinal(State s)
        {
            s = this.validateState(s);
            finals.Add(s);
            return this;
        }

        private State validateState(State st)
        {
            if (!st.getOwner().Equals(guid))
            {
                State imported = states[new State(st, guid, - 1)];
                if (imported == null)
                {
                    throw new Exception("InvalidStateException");
                }
                return imported;
            }
            return states[st];
        }

        public Builder addTransition(State i, char c, State o)
        {
            i = validateState(i);
            o = validateState(o);
            if (Regex.Matches(char.ToString(c), "0-9a-z").Count == 0)
            {
                throw new Exception("Failed to add transition");
            }
            TransitionInput tInput = new TransitionInput(i, c);
            HashSet<State> tOutput = transitions[tInput];
            if (tOutput == null)
            {
                tOutput = new HashSet<State>();
            }

            tOutput.Add(o);
            alphabet.Add(c);
            transitions.Add(tInput, tOutput);
            return this;
        }

        public FiniteAutomata build()
        {
            if (initial == null)
            {
                throw new Exception("Incomplete Automata Exception");
            }

            if (built)
            {
                throw new Exception("Invalid Builder Exception");
            }

            built = true;
            return new FiniteAutomata(this);
        }

    }

    public class TransitionInput
    {
        State s;
        char c;

        public TransitionInput(State s, char c)
        {
            this.s = s;
            this.c = c;
        }

        public State getState()
        {
            return s;
        }
        public char getSymbol()
        {
            return c;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            TransitionInput ti = (TransitionInput)obj;
            return s.Equals(ti.s) && c == ti.c;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
