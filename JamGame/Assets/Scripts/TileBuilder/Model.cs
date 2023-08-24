using Common;
using Level.Room;
using System;
using System.Collections.Generic;
using TileBuilder.Command;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private Transform tileBuilderTransform;

        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private List<CoreModel> roomsInTileBuilder = new();

        private Validator.IValidator validator;

        private void Awake()
        {
            ChangeGameMode(GameMode.God);
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
                    borrowRoom.GetBorrowedRoom.Add(
                        (coreModel) => roomsInTileBuilder.Remove(coreModel)
                    );
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
                GameMode.Play => new Validator.GameMode(),
                _ => throw new ArgumentException(),
            };
        }
    }
}
