using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeController : MonoBehaviour {

    public abstract class Animator
    {
        public Animator() { }
        public abstract void Update();
        public abstract void Start();
        public abstract bool IsDone();
    };

    public class Spiner : Animator
    {
        private Transform _t;
        private Vector3 _axis;
        private float _rate;

        public Spiner(Transform root, Vector3 axis, float rate)
        {
            _t = root;
            _axis = axis;
            _rate = rate;
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            _t.localRotation = Quaternion.AngleAxis(_rate * Time.deltaTime, _axis) * _t.localRotation;
        }

        public override bool IsDone()
        {
            return false;
        }
    }

    public class Pauser : Animator
    {
        private float _delay;
        private float _amount;

        public override void Start()
        {
            _amount = 0.0f;
        }

        public Pauser(float delay)
        {
            _delay = delay;
        }

        public override void Update()
        {
            _amount += Time.deltaTime;
        }

        public override bool IsDone()
        {
            if (_amount > _delay) return true;
            return false;
        }
    }

    public class Rotator : Animator
    {
        private List<Transform> _list;
        private Vector3 _center;
        private Vector3 _axis;
        private float _rate;
        private float _amount;
        private float _delta;
        private List<Quaternion> _targets = new List<Quaternion>();

        public Rotator(List<Transform> list, Vector3 center, Vector3 axis, float amount, float rate = 40)
        {
            _list = list;
            _center = center;
            _axis = axis;
            _amount = amount;
            _rate = rate;
            //Debug.Log("Create " + axis + " " + amount);
        }

        public override void Start()
        {
            _targets.Clear();
            foreach (Transform t in _list)
            {
                Quaternion q = Quaternion.AngleAxis(_amount, _axis) * t.localRotation;
                _targets.Add(q);
            } 
            _delta = 0.0f;
            //Debug.Log(_axis + " " + _amount);
        }

        public override void Update()
        {
            foreach (Transform t in _list)
            {
                t.RotateAround(_center, _axis, _rate * Time.deltaTime);
            }
            _delta += _rate * Time.deltaTime;
        }

        public override bool IsDone()
        {
            if (_delta > _amount)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    Transform t = _list[i] as Transform;
                    t.localRotation = (Quaternion) _targets[i];
                }
                return true;
            }
            return false;
        }
    }
    private List<Animator> rotations = new List<Animator>();
    private int current;

    private CubeStateManager _cube = new CubeStateManager();

	void Start ()
    {
        _cube.init(transform);
    }

    public void ClearQueue()
    {
        rotations.Clear();
        current = 0;
    }

    public void QueueCommand(string word, float turnRate = 30.0f)
    {
        if (word == "") return;

        //Debug.Log(word);

        List<Transform> list;
        Vector3 center;
        Vector3 axis;
        float amount = 0.0f;

        _cube.CmdToTurn(word, out list, out center, out axis, out amount);
        rotations.Add(new Rotator(list, center, axis, amount, turnRate));

        if (rotations.Count == 1) // was previously empty, initialize rotations
        {
            Animator rotator = rotations[0] as Animator;
            rotator.Start();
            current = 0;
        }
    }

    public void QueueSpiner(float turnRate)
    {
        rotations.Add(new Spiner(transform, Vector3.up, turnRate));
    }

    public bool Finished()
    {
        return current >= rotations.Count;
    }

    void Update ()
    {
        if (current >= rotations.Count) return;
        
        Animator rotator = rotations[current] as Animator;
        rotator.Update();
        if (rotator.IsDone())
        {
            _cube.SortCubeGroups();

            //current = (current + 1) % rotations.Count;
            current = current + 1;
            if (current < rotations.Count)
            {
                rotator = rotations[current] as Animator;
                rotator.Start();
            }
        }
	}
}
