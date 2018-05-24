using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t1FormaisGUI
{
    public class State : IEquatable<State>
    {
        private Stack<Guid> owners;
        private Guid guid;
        private int index;

        public State(Guid owner, int index)
        {
            guid = Guid.NewGuid();
            this.index = index;
            owners = new Stack<Guid>();
            owners.Push(owner);

        }

        public State(State state, Guid newOwner, int index)
        {
            guid = state.guid;
            this.index = index;
            //owners = (Stack<Guid>)state.owners.;
            owners.Push(newOwner);
        }
        public bool Equals(State other)
        {
            if (other == null)
            {
                return false;
            }
            State s = other;
            return guid.Equals(s.guid) && owners.Equals(s.owners);
        }

        public Guid getOwner()
        {
            return owners.Peek();
        }

        public override string ToString()
        {
            return "q" + index;
        }

        public override int GetHashCode()
        {
            return 0;
        }

    }
}
