using Common;
using Level.Room;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TileBuilder.Command;
using UnityEngine;
using UnityEngine.Events;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private Transform tileBuilderTransform;

        private ObservableCollection<CoreModel> roomsInTileBuilder = new();
        public UnityEvent<object, NotifyCollectionChangedEventArgs> CollectionChanged = new();

        [SerializeField]
        private TileBuilderImpl tileBuilder;

        private Validator.IValidator validator;

        private void Awake()
        {
            validator = new Validator.GodMode(tileBuilder);
            roomsInTileBuilder.CollectionChanged += CollectionChanged.Invoke;
        }

        public Result Execute(ICommand command)
        {
            Result response = validator.ValidateCommand(command);
            if (response.Success)
            {
                if (command is DropRoom dropRoom)
                {
                    roomsInTileBuilder.Add(dropRoom.CoreModel);
                    dropRoom.CoreModel.transform.parent = tileBuilderTransform;
                }
                else if (command is BorrowRoom borrowRoom)
                {
                    borrowRoom.RoomBorrowed += (coreModel) => roomsInTileBuilder.Remove(coreModel);
                }
                else if (command is RemoveAllRooms remove)
                {
                    foreach (CoreModel room in roomsInTileBuilder)
                    {
                        Destroy(room.gameObject);
                    }
                    roomsInTileBuilder.Clear();
                }
                command.Execute(tileBuilder);
            }
            return response;
        }

        public void ChangeGameMode(GameMode gameMode)
        {
            validator = gameMode switch
            {
                GameMode.God => new Validator.GodMode(tileBuilder),
                GameMode.Build => new Validator.BuildMode(tileBuilder),
                GameMode.Play => new Validator.GameMode(tileBuilder),
                _ => throw new ArgumentException(),
            };
        }
    }
}
